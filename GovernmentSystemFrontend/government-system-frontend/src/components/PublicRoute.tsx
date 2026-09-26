import { Navigate, Outlet } from "react-router-dom";
import { useAuthCheck } from "../hooks/useAuth";

export const PublicRoute = () => {
  const { isAuthenticated, isCheckingAuth } = useAuthCheck();

  // 1. Still waiting to verify session cookie
  if (isCheckingAuth) {
    return (
      <div className="flex h-screen w-full items-center justify-center bg-slate-50">
        <p className="text-sm font-medium text-slate-500">Loading...</p>
      </div>
    );
  }

  // 2. If already logged in, public routes (like /login) are forbidden
  if (isAuthenticated) {
    return <Navigate to="/citizens" replace />;
  }

  // 3. Not logged in -> Show login/register page
  return <Outlet />;
};
