// components/PublicRoute.tsx
import { Navigate, Outlet } from "react-router-dom";
import { useAuthCheck } from "../hooks/useAuth";
import { AuthStatus } from "./AuthStatus";

export const PublicRoute = () => {
  const { isAuthenticated, isCheckingAuth, isAuthError, authError } =
    useAuthCheck();

  if (isCheckingAuth || (isAuthError && authError?.status !== 401)) {
    return (
      <AuthStatus
        isCheckingAuth={isCheckingAuth}
        isAuthError={isAuthError}
        authError={authError}
      />
    );
  }

  if (isAuthenticated) {
    return <Navigate to="/citizens" replace />;
  }

  return <Outlet />;
};
