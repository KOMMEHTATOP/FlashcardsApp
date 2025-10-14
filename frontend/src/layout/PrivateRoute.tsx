import type { JSX } from "react";
import { useApp } from "../context/AppContext";
import { Navigate } from "react-router-dom";

export function PrivateRoute({ children }: { children: JSX.Element }) {
  const { user, loading } = useApp();

  if (loading)
    return (
      <div className="w-full h-screen items-center justify-center flex">
        Loading...
      </div>
    );
  if (!user) return <Navigate to={"/login"} replace />;

  return children;
}
