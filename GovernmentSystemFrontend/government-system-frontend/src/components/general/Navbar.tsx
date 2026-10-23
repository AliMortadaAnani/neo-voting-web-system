// components/Navbar.tsx
import { Link, NavLink } from "react-router-dom";
import { Button } from "./Button";
import type { AuthResponseDTO } from "../../types/auth.types";

export type NavItem = {
  label: string;
  to: string;
};

type NavbarProps = {
  items: NavItem[];
  user?: AuthResponseDTO | null;
  onLogout?: () => void;
  isLoggingOut?: boolean;
};

// Lebanese Flag SVG icon - scales down on mobile (h-5 w-7), standard on desktop (sm:h-6 sm:w-9)
const LebaneseFlagIcon = () => (
  <img
    src="/lebanon-flag.svg"
    alt="Lebanese Flag"
    className="h-5 w-7 rounded border border-stone-200 object-cover shadow-sm transition-all sm:h-6 sm:w-9"
  />
);

export const Navbar = ({
  items,
  user,
  onLogout,
  isLoggingOut,
}: NavbarProps) => {
  return (
    <header className="sticky top-0 z-40 border-b border-stone-200 bg-white px-3 shadow-sm sm:px-6 lg:px-8">
      <div className="mx-auto flex h-16 max-w-7xl items-center justify-between">
        {/* Left Section: Responsive Brand + Nav Links */}
        <div className="flex items-center gap-3 sm:gap-6 md:gap-8">
          <Link to="/" className="flex items-center">
            <span className="font-bold tracking-tight text-red-600">
              {/* Short title on mobile (<640px) */}
              <span className="block text-sm sm:hidden">GOV REG SYSTEM</span>

              {/* Full title on tablets and desktops (>=640px) */}
              <span className="hidden text-sm sm:inline md:text-base">
                Government Registration System
              </span>
            </span>
          </Link>

          {/* Nav links: compact font & tighter spacing on phones */}
          <nav className="flex items-center gap-2 sm:gap-5">
            {items.map((item) => (
              <NavLink
                key={item.to}
                to={item.to}
                state={{ from: window.location.pathname }}
                className={({ isActive }) =>
                  `text-xs font-medium transition-colors sm:text-sm ${
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

        {/* Right Section: User Profile + Logout + Flag */}
        <div className="flex items-center gap-2 sm:gap-4 md:gap-6">
          {user != null && (
            <>
              {/* 
                HIDING THE PROFILE:
                'hidden md:block' hides BOTH the username and the role badge on phones and small tablets.
                It only appears when the screen reaches medium (768px+) width.
              */}
              <div className="hidden text-right md:block">
                <p className="text-xs font-semibold text-stone-800 sm:text-sm">
                  {user.username}
                </p>
                <span className="inline-block rounded-full border border-emerald-200 bg-emerald-50 px-2 py-0.5 text-[10px] font-semibold text-emerald-700">
                  {user.role}
                </span>
              </div>

              {/* Logout Button: Remains visible on all screens */}
              <div className="ml-1">
                <Button
                  label="Logout"
                  onClick={onLogout}
                  color="red"
                  size="sm"
                  isLoading={isLoggingOut}
                />
              </div>
            </>
          )}

          {/* Lebanese Flag icon link to /about */}
          <Link to="/about" className="flex items-center">
            <LebaneseFlagIcon />
          </Link>
        </div>
      </div>
    </header>
  );
};
