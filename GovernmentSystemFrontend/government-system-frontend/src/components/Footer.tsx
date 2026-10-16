// components/Footer.tsx
import { Link } from "react-router-dom";
import type { NavItem } from "./Navbar";

type FooterProps = {
  items: NavItem[];
};

export const Footer = ({ items }: FooterProps) => {
  return (
    <footer className="border-t border-stone-200 bg-white px-4 py-6">
      <div className="mx-auto flex max-w-7xl items-center justify-between gap-4 text-center">
        {/* Info Text */}
        <p className="text-xs font-medium text-gray-500">
          Republic of Lebanon - Mock Government Registration System
        </p>

        {/* Dynamic Footer Links */}
        <div className="flex items-center gap-6">
          {items.map((item) => (
            <Link
              key={item.to}
              to={item.to}
              state={{ from: window.location.pathname }} // Preserve current path for potential redirects
              className="text-xs font-medium text-gray-600 transition-colors hover:text-emerald-700"
            >
              {item.label}
            </Link>
          ))}
        </div>
      </div>
    </footer>
  );
};
