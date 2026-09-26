import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useAuthCheck } from "../hooks/useAuth";

export const ProtectedRoute = () => {
  const location = useLocation();
  const { isAuthenticated, isCheckingAuth, isAuthError, authError } =
    useAuthCheck();

  // 1. Initial session verification
  if (isCheckingAuth) {
    return (
      <div className="flex h-screen w-full items-center justify-center bg-slate-50">
        <p className="text-sm font-medium text-slate-500">
          Checking authentication session...
        </p>
      </div>
    );
  }

  // 2. Server or Network down (not a 401 auth failure)
  if (isAuthError && authError?.status !== 401) {
    return (
      <div className="flex h-screen w-full flex-col items-center justify-center gap-2 bg-slate-50">
        <p className="text-sm font-semibold text-slate-700">
          {authError?.status === 0
            ? "Network error: Unable to reach the server."
            : "Server error: Please try again later."}
        </p>
        <button
          onClick={() => window.location.reload()}
          className="text-xs text-blue-600 underline hover:text-blue-800"
        >
          Reload page
        </button>
      </div>
    );
  }

  // 3. Not logged in or 401 expired cookie -> Redirect to login
  if (!isAuthenticated) {
    // We pass `from: location` so login can redirect them back here
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  // 4. Authorized -> Render protected route content / layout
  return <Outlet />;
};
