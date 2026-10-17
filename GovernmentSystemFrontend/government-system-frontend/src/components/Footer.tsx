// components/Footer.tsx
import { Link } from "react-router-dom";
import type { NavItem } from "./Navbar";

type FooterProps = {
  items: NavItem[];
};

export const Footer = ({ items }: FooterProps) => {
  return (
    <footer className="border-t border-stone-200 bg-white px-4 py-6">
      {/* 
        RESPONSIVE CONTAINER:
        - Mobile (< 640px): 'flex flex-col text-center' -> Stacks info and links vertically
        - Tablet/Desktop (sm: >= 640px): 'sm:flex-row sm:text-left' -> Side-by-side row
      */}
      <div className="mx-auto flex max-w-7xl flex-col items-center justify-between gap-4 text-center sm:flex-row sm:text-left">
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
              state={{ from: window.location.pathname }}
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
