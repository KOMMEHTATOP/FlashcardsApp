import React, {
    createContext,
    useCallback,
    useContext,
    useEffect,
    useState,
} from "react";
import { useNavigate } from "react-router-dom";
import apiFetch, { setUnauthorizedCallback, setAuthToken, removeAuthToken } from "@/utils/apiFetch";

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
                await apiFetch.get("/Auth/validate", { signal: controller.signal });
                setIsAuthenticated(true);
            } catch (err: any) {
                if (err.name !== 'AbortError' && err.name !== 'CanceledError') {
                    removeAuthToken();
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

    useEffect(() => {
        const handleUnauthorized = () => {
            setIsAuthenticated(false);
            navigate("/login", { replace: true });
        };

        setUnauthorizedCallback(handleUnauthorized);
    }, [navigate]);

    const login = useCallback(
        async (email: string, password: string) => {
            const response = await apiFetch.post("/Auth/login", {
                email,
                password,
            });

            const token = response.data.accessToken || response.data.AccessToken;

            if (!token) {
                throw new Error("Токен не получен от сервера при входе.");
            }

            setAuthToken(token);
            setIsAuthenticated(true);
        },
        []
    );

    const register = useCallback(
        async (loginName: string, email: string, password: string) => {
            await apiFetch.post("/Auth/register", {
                Login: loginName,
                Email: email,
                Password: password,
            });

            await login(email, password);
        },
        [login] 
    );

    const logout = useCallback(async () => {
        try {
            await apiFetch.post("/Auth/logout");
        } catch (err) {
            console.error("Logout error:", err);
        } finally {
            removeAuthToken();
            setIsAuthenticated(false);
            navigate("/login", { replace: true });
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