// layouts/PublicLayout.tsx
import React from "react";
import { Navbar, type NavItem } from "../components/Navbar";
import { Footer } from "../components/Footer";

type PublicLayoutProps = {
  children: React.ReactNode;
};

export const PublicLayout = ({ children }: PublicLayoutProps) => {
  // Public Links
  const navItems: NavItem[] = [
    { label: "Login", to: "/login" },
    { label: "About", to: "/about" },
  ];

  return (
    <div className="flex min-h-screen flex-col justify-between bg-gray-50">
      {/* Reusable Navbar configured for Public */}
      <Navbar items={navItems} />

      {/* Main Content Area */}
      <main className="flex flex-1 items-center justify-center p-4">
        {children}
      </main>

      {/* Reusable Footer */}
      <Footer items={navItems} />
    </div>
  );
};
