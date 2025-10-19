import type { JSX } from "react";
import { useApp } from "../context/AppContext";
import { Navigate, useLocation } from "react-router-dom";

export function GuestRoute({ children }: { children: JSX.Element }) {
  const { user, loading } = useApp();
  const location = useLocation();

  if (loading)
    return (
      <div className="w-full h-full min-h-screen flex justify-center items-center">
        <div className="loading loading-dots loading-lg"></div>
      </div>
    );

  if (
    user &&
    ["/login", "/register", "/forgot-password"].includes(location.pathname)
  ) {
    return <Navigate to="/" replace />;
  }

  return children;
}
