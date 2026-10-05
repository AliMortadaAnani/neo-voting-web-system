// layouts/AppLayout.tsx
import { NavLink, Outlet } from "react-router-dom";
import { useAuth, useAuthCheck } from "../hooks/useAuth";

export const AppLayout = () => {
  const { logout, isLoggingOut } = useAuth();
  const { user } = useAuthCheck();

  // Helper for active link styles
  const navLinkClass = ({ isActive }: { isActive: boolean }) =>
    `text-sm font-medium transition-colors ${
      isActive
        ? "text-blue-600 dark:text-blue-400 font-semibold"
        : "text-gray-600 hover:text-gray-900 dark:text-zinc-400 dark:hover:text-zinc-100"
    }`;

  return (
    <div className="min-h-screen bg-gray-50 text-gray-900 antialiased dark:bg-zinc-950 dark:text-zinc-100 flex flex-col">
      {/* Top Navbar */}
      <header className="sticky top-0 z-40 border-b border-gray-200 bg-white/80 backdrop-blur-md dark:border-zinc-800 dark:bg-zinc-900/80">
        <div className="mx-auto flex h-16 max-w-7xl items-center justify-between px-4 sm:px-6 lg:px-8">
          {/* Logo & Navigation */}
          <div className="flex items-center gap-8">
            <span className="text-lg font-bold tracking-tight text-blue-600 dark:text-blue-500">
              GovPortal
            </span>
            <nav className="flex items-center gap-6">
              <NavLink to="/citizens" className={navLinkClass}>
                Citizens
              </NavLink>
              <NavLink to="/voters" className={navLinkClass}>
                Voters
              </NavLink>
              <NavLink to="/candidates" className={navLinkClass}>
                Candidates
              </NavLink>
            </nav>
          </div>

          {/* User Info & Actions */}
          <div className="flex items-center gap-4">
            <div className="text-right">
              <p className="text-sm font-semibold">{user?.username}</p>
              <span className="inline-block rounded-full bg-blue-100 px-2 py-0.5 text-xs font-medium text-blue-800 dark:bg-blue-900/30 dark:text-blue-300">
                {user?.role}
              </span>
            </div>

            <button
              onClick={() => logout()}
              disabled={isLoggingOut}
              className="rounded-lg border border-gray-300 px-3 py-1.5 text-xs font-medium text-gray-700 hover:bg-gray-100 disabled:opacity-50 dark:border-zinc-700 dark:text-zinc-300 dark:hover:bg-zinc-800 transition"
            >
              {isLoggingOut ? "Logging out..." : "Logout"}
            </button>
          </div>
        </div>
      </header>

      {/* Main Content Area */}
      <main className="mx-auto w-full max-w-7xl flex-1 px-4 py-6 sm:px-6 lg:px-8">
        <Outlet />
      </main>
    </div>
  );
};
