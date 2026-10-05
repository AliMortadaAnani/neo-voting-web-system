// components/ProtectedRoute.tsx
import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useAuthCheck } from "../hooks/useAuth";
import { AuthStatus } from "./AuthStatus";

export const ProtectedRoute = () => {
  const location = useLocation();
  const { isAuthenticated, isCheckingAuth, isAuthError, authError } =
    useAuthCheck();

  // Show loading / error UI first
  if (isCheckingAuth || (isAuthError && authError?.status !== 401)) {
    return (
      <AuthStatus
        isCheckingAuth={isCheckingAuth}
        isAuthError={isAuthError}
        authError={authError}
      />
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  return <Outlet />;
};
