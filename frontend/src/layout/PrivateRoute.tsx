import type { JSX } from "react";
import { useAuth } from "@/context/AuthContext";
import { Navigate } from "react-router-dom";
import SkeletonHome from "@/components/SkeletonHome";
import { DEV } from "@/App";

export function PrivateRoute({ children }: { children: JSX.Element }) {
    const { isAuthenticated, isLoading } = useAuth();

    if (isLoading) return <SkeletonHome />;

    if (!DEV) {
        if (!isAuthenticated) return <Navigate to={"/about"} replace />;
    }

    return children;
}