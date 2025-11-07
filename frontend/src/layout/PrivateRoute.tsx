import type { JSX } from "react";
import { useApp } from "../context/AppContext";
import { Navigate } from "react-router-dom";
import SkeletonHome from "../components/SkeletonHome";
import { DEV } from "../App";

export function PrivateRoute({ children }: { children: JSX.Element }) {
  const { user, loading } = useApp();

  if (loading) return <SkeletonHome />;
  if (!DEV) {
    if (!user) return <Navigate to={"/about"} replace />;
  }

  return children;
}
