// components/ProtectedRoute.tsx
import { Navigate, Outlet } from "react-router-dom";
import { useAuthCheck } from "../../hooks/useAuth";
import { ServerErrorState } from "./ServerErrorState";
import { ProtectedLayout } from "./ProtectedLayout";
import { LoadingSpinner } from "./LoadingSpinner";

export const ProtectedRoute = () => {
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

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  } // state={{ from: location }} is used to redirect the user back to the page they were trying to access after they log in // LATER

  return (
    <>
      <ProtectedLayout>
        <Outlet />
      </ProtectedLayout>
    </>
  );
};
