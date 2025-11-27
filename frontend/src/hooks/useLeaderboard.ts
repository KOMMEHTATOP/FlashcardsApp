import { useEffect, useState, useCallback, useRef } from 'react';
import * as signalR from '@microsoft/signalr';
import axios from 'axios';
import type { LeaderboardResponseDto } from '@/types/leaderboard';

const API_URL = import.meta.env.VITE_API_URL;

export const useLeaderboard = () => {
    const [data, setData] = useState<LeaderboardResponseDto | null>(null);
    const [loading, setLoading] = useState(true);

    // Используем useRef, чтобы хранить соединение между рендерами, не вызывая их
    const connectionRef = useRef<signalR.HubConnection | null>(null);

    const fetchData = useCallback(async () => {
        const token = localStorage.getItem('accessToken');
        if (!token) return;

        try {
            const response = await axios.get(`${API_URL}/leaderboard`, {
                headers: { Authorization: `Bearer ${token}` }
            });
            // console.log("📊 Leaderboard refreshed via API"); // Можно оставить для отладки, но лучше убрать в проде
            setData(response.data);
        } catch (error) {
            console.error('Failed to fetch leaderboard:', error);
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        const token = localStorage.getItem('accessToken');
        if (!token) {
            setLoading(false);
            return;
        }

        // 1. Грузим данные сразу (REST)
        fetchData();

        // 2. Настраиваем SignalR
        const connection = new signalR.HubConnectionBuilder()
            .withUrl(`${API_URL}/notificationHub`, {
                accessTokenFactory: () => token,
                skipNegotiation: true,
                transport: signalR.HttpTransportType.WebSockets
            })
            .withAutomaticReconnect()
            // ВАЖНО: Уровень Warning скроет url с токеном из консоли
            .configureLogging(signalR.LogLevel.Warning)
            .build();

        connection.on('LeaderboardUpdated', () => {
            // console.log("🔔 Leaderboard update signal received");
            fetchData();
        });

        // Флаг для предотвращения Race Condition в React Strict Mode
        let isMounted = true;

        const startConnection = async () => {
            try {
                // Если компонент уже размонтирован к моменту старта - не начинаем
                if (!isMounted) return;

                await connection.start();

                if (isMounted) {
                    console.log('🟢 SignalR Connected');
                    // Сохраняем активное соединение в ref только после успешного старта
                    connectionRef.current = connection;
                } else {
                    // Если пока грузились, компонент умер - сразу отключаемся
                    connection.stop();
                }
            } catch (err: any) {
                // Игнорируем ошибку отмены (AbortError), это нормально при быстрой перезагрузке
                if (err.toString().includes("AbortError")) return;
                console.error('🔴 SignalR Connection Error:', err);
            }
        };

        startConnection();

        return () => {
            isMounted = false;
            // Останавливаем только если соединение было создано и запущено
            if (connectionRef.current) {
                connectionRef.current.stop();
                connectionRef.current = null;
            }
            // Если startConnection еще висит в await, флаг isMounted=false не даст ему сохранить соединение
        };
    }, [fetchData]);

    return { data, loading };
};