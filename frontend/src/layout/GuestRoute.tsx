import type { JSX } from "react";
import { useApp } from "../context/AppContext";
import { Navigate } from "react-router-dom";

export function GuestRoute({ children }: { children: JSX.Element }) {
  const { user, loading } = useApp();

  if (loading)
    return (
      <div className="w-full h-full min-h-screen flex justify-center items-center">
        <div className="loading loading-dots loading-lg"></div>
      </div>
    );
  if (!user) return <Navigate to={"/"} replace />;

  return children;
}
