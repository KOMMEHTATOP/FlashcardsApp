import React, {
    createContext,
    useCallback,
    useContext,
    useEffect,
    useState,
} from "react";
import { useNavigate } from "react-router-dom";
import apiFetch, { setUnauthorizedCallback } from "../utils/apiFetch";

interface AuthContextType {
    isAuthenticated: boolean;
    isLoading: boolean;
    login: (email: string, password: string) => Promise<void>;
    register: (login: string, email: string, password: string) => Promise<void>;
    logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | null>(null);

export function AuthProvider({ children }: { children: React.ReactNode }) {
    const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const navigate = useNavigate();

    // Проверка токена при монтировании
    useEffect(() => {
        const controller = new AbortController();

        const checkAuth = async () => {
            const token = localStorage.getItem("accessToken");

            if (!token) {
                setIsAuthenticated(false);
                setIsLoading(false);
                return;
            }

            try {
                // Делаем легкий запрос для проверки валидности токена
                await apiFetch.get("/Auth/validate", { signal: controller.signal });
                setIsAuthenticated(true);
            } catch (err: any) {
                // Если ошибка не из-за отмены запроса
                if (err.name !== 'AbortError' && err.name !== 'CanceledError') {
                    localStorage.removeItem("accessToken");
                    setIsAuthenticated(false);
                }
            } finally {
                if (!controller.signal.aborted) {
                    setIsLoading(false);
                }
            }
        };

        checkAuth();

        return () => {
            controller.abort();
        };
    }, []);

    // Устанавливаем callback для interceptor
    useEffect(() => {
        const handleUnauthorized = () => {
            setIsAuthenticated(false);
            navigate("/about", { replace: true });
        };

        setUnauthorizedCallback(handleUnauthorized);
    }, [navigate]);

    const login = useCallback(
        async (email: string, password: string) => {
            const response = await apiFetch.post("/Auth/login", {
                email,
                password,
            });

            const token = response.data.accessToken;
            localStorage.setItem("accessToken", token);

            setIsAuthenticated(true);

            // Используем setTimeout чтобы дать React обновить state перед навигацией
            setTimeout(() => {
                navigate("/", { replace: true });
            }, 0);
        },
        [navigate]
    );

    const register = useCallback(
        async (login: string, email: string, password: string) => {
            const response = await apiFetch.post("/Auth/register", {
                Login: login,
                Email: email,
                Password: password,
            });

            const token = response.data.accessToken;
            localStorage.setItem("accessToken", token);

            setIsAuthenticated(true);

            setTimeout(() => {
                navigate("/", { replace: true });
            }, 0);
        },
        [navigate]
    );
    
    
    const logout = useCallback(async () => {
        try {
            await apiFetch.post("/Auth/logout");
        } catch (err) {
            console.error("Logout error:", err);
        } finally {
            localStorage.removeItem("accessToken");
            setIsAuthenticated(false);
            navigate("/about", { replace: true });
        }
    }, [navigate]);

    const value: AuthContextType = {
        isAuthenticated,
        isLoading,
        login,
        register,
        logout,
    };

    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error("useAuth must be used within AuthProvider");
    }
    return context;
}