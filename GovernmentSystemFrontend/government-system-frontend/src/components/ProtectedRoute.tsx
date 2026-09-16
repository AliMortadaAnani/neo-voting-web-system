import { Navigate, Outlet, Link } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";

export const ProtectedRoute = () => {
  const { isAuthenticated, isCheckingAuth, user, logoutAsync, isLoggingOut } =
    useAuth();

  if (isCheckingAuth) {
    return (
      <div className="flex h-screen w-full items-center justify-center bg-slate-50">
        <p className="text-sm font-medium text-slate-500">
          Checking authentication session...
        </p>
      </div>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return (
    <div className="min-h-screen bg-slate-50 text-slate-900">
      {/* Shared Admin Header */}
      <header className="border-b border-slate-200 bg-white px-6 py-3.5 shadow-xs">
        <div className="mx-auto flex max-w-7xl items-center justify-between">
          {/* Brand & Entity Navigation */}
          <div className="flex items-center gap-8">
            <span className="font-bold tracking-tight text-slate-900">
              Government Portal
            </span>
            <nav className="flex items-center gap-4 text-sm font-medium text-slate-600">
              <Link
                to="/citizens"
                className="transition-colors hover:text-slate-900"
              >
                Citizens
              </Link>
              <Link
                to="/voters"
                className="transition-colors hover:text-slate-900"
              >
                Voters
              </Link>
              <Link
                to="/candidates"
                className="transition-colors hover:text-slate-900"
              >
                Candidates
              </Link>
            </nav>
          </div>

          {/* User Badge & Logout */}
          <div className="flex items-center gap-4">
            <span className="rounded-full bg-slate-100 px-3 py-1 text-xs font-medium text-slate-600">
              {user?.username} ({user?.role})
            </span>
            <button
              onClick={() => logoutAsync()}
              disabled={isLoggingOut}
              className="rounded-lg border border-slate-300 px-3 py-1.5 text-xs font-semibold text-slate-700 transition-colors hover:bg-slate-100 disabled:opacity-50"
            >
              {isLoggingOut ? "Logging out..." : "Logout"}
            </button>
          </div>
        </div>
      </header>

      {/* Page Content */}
      <main className="mx-auto max-w-7xl p-6">
        <Outlet />
      </main>
    </div>
  );
};
