// components/PublicRoute.tsx
import { Navigate, Outlet } from "react-router-dom";
import { useAuthCheck } from "../../hooks/useAuth";
import { ServerErrorState } from "./ServerErrorState";
import { LoadingSpinner } from "./LoadingSpinner";
import { PublicLayout } from "./PublicLayout";

export const PublicRoute = () => {
  const { isAuthenticated, isCheckingAuth, isAuthError, authError } =
    useAuthCheck();

  if (isCheckingAuth) {
    return (
      <LoadingSpinner label="Checking authentication..." fullScreen={true} />
    );
  }

  // if the error is not a 401 error, we will show the error state (general error page)
  if (isAuthError && authError?.status !== 401) {
    return <ServerErrorState error={authError} fullScreen={true} />;
  }

  if (isAuthenticated) {
    return <Navigate to="/citizens" replace />;
  }
  return (
    <>
      <PublicLayout>
        <Outlet />
      </PublicLayout>
    </>
  );
};
