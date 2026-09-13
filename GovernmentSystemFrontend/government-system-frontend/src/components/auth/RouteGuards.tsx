import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../../hooks/useAuth";

//protects routes that require authentication
export const ProtectedRoute = () => {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) {
    return (
      <div className="flex h-screen items-center justify-center bg-gray-900 text-white">
        <p className="animate-pulse text-lg">Loading session...</p>
      </div>
    );
  }

  return isAuthenticated ? <Outlet /> : <Navigate to="/login" replace />;
};

// Use this for the Login page (so logged-in users can't see the login screen)
export const PublicRoute = () => {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) {
    return (
      <div className="flex h-screen items-center justify-center bg-gray-900 text-white">
        <p className="animate-pulse text-lg">Loading...</p>
      </div>
    );
  }

  return !isAuthenticated ? <Outlet /> : <Navigate to="/home" replace />;
};
