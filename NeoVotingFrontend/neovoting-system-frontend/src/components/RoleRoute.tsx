import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import { ROLE_HOME } from "../utils/jwt";
import type { Role } from "../types/auth.type";

type RoleRouteProps = {
  allowedRole: Role;
};

export const RoleRoute = ({ allowedRole }: RoleRouteProps) => {
  const { user, role, isAuthenticated, isCheckingAuth, logout, isLoggingOut } =
    useAuth();

  if (isCheckingAuth) {
    return (
      <div className="flex h-screen w-full items-center justify-center bg-slate-50">
        <p className="text-sm font-medium text-slate-500">
          Checking authorization...
        </p>
      </div>
    );
  }

  // 1. Not logged in -> Redirect to login
  if (!isAuthenticated || !role) {
    return <Navigate to="/login" replace />;
  }

  // 2. Role mismatch -> Bounce to their own home portal
  if (role !== allowedRole) {
    return <Navigate to={ROLE_HOME[role]} replace />;
  }

  // 3. Authorized -> Render Shared Header with Logout & Page Content
  return (
    <div className="min-h-screen bg-slate-50 text-slate-900">
      <header className="border-b border-slate-200 bg-white px-6 py-3.5 shadow-xs">
        <div className="mx-auto flex max-w-7xl items-center justify-between">
          {/* Brand */}
          <div className="flex items-center gap-4">
            <span className="font-bold tracking-tight text-slate-900">
              Voting Portal
            </span>
            <span className="rounded-md bg-slate-100 px-2 py-0.5 text-xs font-semibold text-slate-600 uppercase">
              {role} Panel
            </span>
          </div>

          {/* User Info & Logout Button */}
          <div className="flex items-center gap-4">
            <span className="text-sm font-medium text-slate-700">
              {user?.username}
              {" - "}
              {user?.accountId ?? "none"}
              {" - "}
              {user?.appUserId}
            </span>
            <button
              onClick={() => logout()}
              disabled={isLoggingOut}
              className="rounded-lg border border-slate-300 px-3 py-1.5 text-xs font-semibold text-slate-700 transition-colors hover:bg-slate-100 disabled:opacity-50"
            >
              {isLoggingOut ? "Logging out..." : "Logout"}
            </button>
          </div>
        </div>
      </header>

      <main className="mx-auto max-w-7xl p-6">
        <Outlet />
      </main>
    </div>
  );
};
