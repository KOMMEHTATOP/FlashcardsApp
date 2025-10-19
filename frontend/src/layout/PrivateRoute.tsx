import type { JSX } from "react";
import { useApp } from "../context/AppContext";
import { Navigate } from "react-router-dom";
import SkeletonHome from "../components/SkeletonHome";

export function PrivateRoute({ children }: { children: JSX.Element }) {
  const { user, loading } = useApp();

  if (loading) return <SkeletonHome />;
  if (!user) return <Navigate to={"/login"} replace />;

  return children;
}
