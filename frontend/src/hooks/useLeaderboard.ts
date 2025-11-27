import { useEffect, useState, useCallback, useRef } from 'react';
import * as signalR from '@microsoft/signalr';
import axios from 'axios';
import type { LeaderboardResponseDto } from '@/types/leaderboard';

const API_URL = import.meta.env.VITE_API_URL;

export const useLeaderboard = () => {
    const [data, setData] = useState<LeaderboardResponseDto | null>(null);
    const [loading, setLoading] = useState(true);
    const connectionRef = useRef<signalR.HubConnection | null>(null);

    const fetchData = useCallback(async () => {
        const token = localStorage.getItem('accessToken');
        if (!token) return;

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
    }, []);

    useEffect(() => {
        const token = localStorage.getItem('accessToken');
        if (!token) {
            setLoading(false);
            return;
        }

        fetchData();

        // Настраиваем SignalR
        const connection = new signalR.HubConnectionBuilder()
            .withUrl(`${API_URL}/notificationHub`, {
                accessTokenFactory: () => token,
                skipNegotiation: true,
                transport: signalR.HttpTransportType.WebSockets
            })
            .withAutomaticReconnect()
            // Warning скроет url с токеном из консоли
            .configureLogging(signalR.LogLevel.Warning)
            .build();

        connection.on('LeaderboardUpdated', () => {
            fetchData();
        });

        let isMounted = true;

        const startConnection = async () => {
            try {
                if (!isMounted) return;

                await connection.start();

                if (isMounted) {
                    console.log('SignalR Connected');
                    connectionRef.current = connection;
                } else {
                    connection.stop();
                }
            } catch (err: any) {
                if (err.toString().includes("AbortError")) return;
                console.error('SignalR Connection Error:', err);
            }
        };

        startConnection();

        return () => {
            isMounted = false;
            if (connectionRef.current) {
                connectionRef.current.stop();
                connectionRef.current = null;
            }
        };
    }, [fetchData]);

    return { data, loading };
};