import { useEffect, useState, useRef } from 'react';
import * as signalR from '@microsoft/signalr';
import axios from 'axios';
import type { LeaderboardResponseDto } from '@/types/leaderboard';

const API_URL = import.meta.env.VITE_API_URL;

export const useLeaderboard = () => {
    const [data, setData] = useState<LeaderboardResponseDto | null>(null);
    const [loading, setLoading] = useState(true);

    // Нам нужен только ref для самого соединения
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
        // Создаем новый инстанс
        const connection = new signalR.HubConnectionBuilder()
            .withUrl(`${API_URL}/notificationHub`, {
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Warning)
            .build();

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
                // АНАЛИЗ ПРОБЛЕМЫ:
                // React Strict Mode прерывает соединение в момент "negotiation" (согласования).
                // Это нормальное поведение для dev-режима, ошибку нужно игнорировать.
                if (
                    errorMessage.includes("AbortError") ||
                    errorMessage.includes("invocation cancelled") ||
                    errorMessage.includes("negotiation") // <--- Добавлено ключевое слово из твоей ошибки
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
            // При размонтировании останавливаем соединение
            // Важно: SignalR сам выбросит ошибку, если мы остановим его во время старта,
            // но мы перехватим её в catch блоке выше.
            connection.stop();
        };
    }, []);

    return { data, loading };
};