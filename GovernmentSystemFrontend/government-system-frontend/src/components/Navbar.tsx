import { Link, NavLink } from "react-router-dom";
import { Button } from "./Button";
import type { AuthResponseDTO } from "../types/auth.types";

export type NavItem = {
  label: string;
  to: string;
};

type NavbarProps = {
  items: NavItem[];
  user?: AuthResponseDTO | null;
  LogoutAction?: () => void;
  isLoggingOut?: boolean;
};

// Official Lebanese Flag SVG icon
const LebaneseFlagIcon = () => (
  <img
    src="/lebanon-flag.svg"
    alt="Lebanese Flag"
    className="h-6 w-9 rounded border border-stone-100 object-cover shadow-sm"
  />
);

export const Navbar = ({
  items,
  user,
  LogoutAction,
  isLoggingOut,
}: NavbarProps) => {
  return (
    <header className="sticky top-0 z-40 border-b border-stone-200 bg-white pr-2 pl-1 shadow-sm">
      <div className="mx-auto flex h-16 max-w-7xl items-center justify-between">
        {/* Left Side: Flag Logo + Dynamic Nav Links */}
        <div className="mt-0.5 flex items-center gap-10">
          <Link to="/">
            <span className="text-lg font-bold tracking-tight text-red-600">
              Government Registration System
            </span>
          </Link>

          {/* Dynamic Navigation Links */}
          <nav className="flex items-center gap-6">
            {items.map((item) => (
              <NavLink
                key={item.to}
                to={item.to}
                state={{ from: window.location.pathname }} // Preserve current path for potential redirects
                className={({ isActive }) =>
                  `text-sm font-medium transition-colors ${
                    isActive
                      ? "border-b-2 border-emerald-600 pb-0.5 font-bold text-emerald-700"
                      : "text-gray-600 hover:text-emerald-600"
                  }`
                }
              >
                {item.label}
              </NavLink>
            ))}
          </nav>
        </div>

        {/* Right Side: User Info + Logout Button */}
        <div className="flex items-center gap-7">
          {user != null && (
            <>
              <div className="text-right">
                <p className="text-sm font-semibold text-stone-800">
                  {user.username}
                </p>
                <span className="inline-block rounded-full border border-emerald-200 bg-emerald-50 px-2 py-0.5 text-xs font-semibold text-emerald-700">
                  {user.role}
                </span>
              </div>
              <Button
                label="Logout"
                onClick={LogoutAction}
                color="red"
                size="sm"
                isLoading={isLoggingOut}
              />
            </>
          )}

          <Link to="/about">
            <LebaneseFlagIcon />
          </Link>
        </div>
      </div>
    </header>
  );
};
