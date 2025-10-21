import axios from "axios";
const BASE_URL = import.meta.env.VITE_API_URL || "http://localhost:5001/api";
export { BASE_URL };
const apiFetch = axios.create({
    baseURL: BASE_URL,
    withCredentials: true
});

let isRefreshing = false;
let failedQueue: any = [];


const proccesQuene = (error : any, token =null) => {
    failedQueue.forEach((prom: any) => {
        if (error) prom.reject(error);
        else prom.resolve(token);
    });
    failedQueue = [];
}

apiFetch.interceptors.request.use((config) => {
    const token = localStorage.getItem("accessToken");
    if (token) config.headers["Authorization"] = `Bearer ${token}`;
    return config;
})

apiFetch.interceptors.response.use((response) => 
    response,
    async (error) => {
        const originalRequest = error.config;
        
        if (error.response && error.response.status === 401 && !originalRequest._retry) {
            originalRequest._retry = true;
            if (isRefreshing) {
                return new Promise((resolve, reject) => {
                    failedQueue.push({ resolve, reject });
                })
                .then((token) => {
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
                    {withCredentials: true} 
                );

                const newToken = response?.data.accessToken;
                localStorage.setItem("accessToken", newToken);

                apiFetch.defaults.headers.common["Authorization"] = `Bearer ${newToken}`;


                proccesQuene(null, newToken);
                return apiFetch(originalRequest);
            } catch (err) {
                proccesQuene(err);
                return Promise.reject(err);
            } finally {
                isRefreshing = false;
            }
        }
        
        return Promise.reject(error);
}
);



export default apiFetch;