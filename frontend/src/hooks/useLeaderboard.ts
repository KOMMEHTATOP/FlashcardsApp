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

        // конфигурация SignalR
        const connection = new signalR.HubConnectionBuilder()
            .withUrl(`${API_URL}/notificationHub`, {
                accessTokenFactory: () => token
            })
            .withServerTimeout(120000) // 120 секунд - КРИТИЧНО!
            .withKeepAliveInterval(15000) // 15 секунд
            .withAutomaticReconnect([0, 2000, 10000, 30000])
            .configureLogging(signalR.LogLevel.Warning)
            .build();

        connection.on('LeaderboardUpdated', (updatedData: LeaderboardResponseDto) => {
            setData(updatedData);
        });

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

        return () => {
            connection.stop();
        };
    }, []);

    return { data, loading };
};