import { useEffect, useState, useRef } from 'react';
import * as signalR from '@microsoft/signalr';
import axios from 'axios';
import type { LeaderboardResponseDto } from '@/types/leaderboard';

const API_URL = import.meta.env.VITE_API_URL;

export const useLeaderboard = () => {
    const [data, setData] = useState<LeaderboardResponseDto | null>(null);
    const [loading, setLoading] = useState(true);
    const connectionRef = useRef<signalR.HubConnection | null>(null);

    useEffect(() => {
        const token = localStorage.getItem('accessToken');

        if (!token) {
            setLoading(false);
            return;
        }

        // 1. REST запрос
        const fetchInitialData = async () => {
            try {
                const response = await axios.get(`${API_URL}/leaderboard`, {
                    headers: { Authorization: `Bearer ${token}` }
                });
                setData(response.data);
            } catch (error) {
                console.error('Failed to fetch leaderboard:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchInitialData();

        // 2. SignalR Подключение
        const connection = new signalR.HubConnectionBuilder()
            .withUrl(`${API_URL}/notificationHub`, {
                accessTokenFactory: () => token,
                timeout: 120000 // 120 секунд
            })
            .withAutomaticReconnect([0, 2000, 10000, 30000])
            .configureLogging(signalR.LogLevel.Warning)
            .build();

        // ДОБАВЛЕНО: Настройка таймаутов
        connection.serverTimeoutInMilliseconds = 120000; // 120 секунд
        connection.keepAliveIntervalInMilliseconds = 15000; // 15 секунд

        connection.on('LeaderboardUpdated', (updatedData: LeaderboardResponseDto) => {
            setData(updatedData);
        });

        // Функция старта
        const startConnection = async () => {
            try {
                await connection.start();
                console.log('🟢 SignalR Connected');
            } catch (err: any) {
                const errorMessage = err.toString();
                if (
                    errorMessage.includes("AbortError") ||
                    errorMessage.includes("invocation cancelled") ||
                    errorMessage.includes("negotiation")
                ) {
                    return;
                }
                console.error('🔴 SignalR Connection Error:', err);
            }
        };

        startConnection();
        connectionRef.current = connection;

        // Cleanup
        return () => {
            connection.stop();
        };
    }, []);

    return { data, loading };
};