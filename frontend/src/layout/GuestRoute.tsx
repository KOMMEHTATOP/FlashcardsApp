import type { JSX } from "react";
import { useAuth } from "../context/AuthContext";
import { Navigate, useLocation } from "react-router-dom";

export function GuestRoute({ children }: { children: JSX.Element }) {
    const { isAuthenticated, isLoading } = useAuth();
    const location = useLocation();

    if (isLoading)
        return (
            <div className="w-full h-full min-h-screen flex justify-center items-center">
                <div className="loading loading-dots loading-lg"></div>
            </div>
        );

    if (
        isAuthenticated &&
        ["/login", "/register", "/forgot-password"].includes(location.pathname)
    ) {
        return <Navigate to="/" replace />;
    }

    return children;
}