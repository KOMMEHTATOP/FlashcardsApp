import axios from "axios";

const BASE_URL = import.meta.env.VITE_API_URL || "https://localhost:7112/api";
export { BASE_URL };

const apiFetch = axios.create({
    baseURL: BASE_URL,
    withCredentials: true
});

let isRefreshing = false;
let failedQueue: any = [];

let onUnauthorized: (() => void) | null = null;

export const setUnauthorizedCallback = (callback: () => void) => {
    onUnauthorized = callback;
};

const initialToken = localStorage.getItem("accessToken");
if (initialToken) {
    apiFetch.defaults.headers.common["Authorization"] = `Bearer ${initialToken}`;
}

const processQueue = (error: any, token = null) => {
    failedQueue.forEach((prom: any) => {
        if (error) prom.reject(error);
        else prom.resolve(token);
    });
    failedQueue = [];
};

export const setAuthToken = (token: string) => {
    localStorage.setItem("accessToken", token);
    apiFetch.defaults.headers.common["Authorization"] = `Bearer ${token}`;
};

export const removeAuthToken = () => {
    localStorage.removeItem("accessToken");
    delete apiFetch.defaults.headers.common["Authorization"];
};

apiFetch.interceptors.request.use((config) => {
    const token = localStorage.getItem("accessToken");
    if (token && !config.headers["Authorization"]) {
        config.headers["Authorization"] = `Bearer ${token}`;
    }
    return config;
});

apiFetch.interceptors.response.use(
    (response) => response,
    async (error) => {
        const originalRequest = error.config;

        if (error.response && error.response.status === 401 && !originalRequest._retry) {
            originalRequest._retry = true;

            if (isRefreshing) {
                console.log("-> Refresh already in progress. Queuing request:", originalRequest.url); 
                return new Promise((resolve, reject) => {
                    failedQueue.push({ resolve, reject });
                })
                    .then((token) => {
                        // Защитный механизм для заголовков
                        if (!originalRequest.headers) {
                            originalRequest.headers = {};
                        }

                        originalRequest.headers["Authorization"] = `Bearer ${token}`;
                        return apiFetch(originalRequest);
                    })
                    .catch(Promise.reject);
            }

            isRefreshing = true;

            try {
                const response = await axios.post(
                    `${BASE_URL}/Auth/refresh`,
                    {},
                    { withCredentials: true }
                );

                // ИСПРАВЛЕНИЕ КЛЮЧА
                const newToken = response?.data.accessToken;

                if (!newToken) {
                    throw new Error("Refresh token failed: Missing Access Token in response");
                }

                setAuthToken(newToken); 
                processQueue(null, newToken); 

                originalRequest.headers["Authorization"] = `Bearer ${newToken}`;
                
                return apiFetch(originalRequest);
            } catch (err) {
                processQueue(err);
                removeAuthToken();

                if (onUnauthorized) {
                    onUnauthorized();
                }

                return Promise.reject(err);
            } finally {
                isRefreshing = false;
            }
        }

        return Promise.reject(error);
    }
);

export default apiFetch;