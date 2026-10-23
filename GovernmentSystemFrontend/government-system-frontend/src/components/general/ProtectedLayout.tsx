// layouts/ProtectedLayout.tsx
import React from "react";
import { Navbar, type NavItem } from "../../components/general/Navbar";
import { Footer } from "../../components/general/Footer";
import { useAuth, useAuthCheck } from "../../hooks/useAuth";
import { LoadingSpinner } from "../../components/general/LoadingSpinner";

type ProtectedLayoutProps = {
  children: React.ReactNode;
};

export const ProtectedLayout = ({ children }: ProtectedLayoutProps) => {
  const { logout, isLoggingOut } = useAuth();
  const { user } = useAuthCheck();

  // Protected Admin Navigation Links
  const protectedItems: NavItem[] = [
    { label: "Citizens", to: "/citizens" },
    { label: "Voters", to: "/voters" },
    { label: "Candidates", to: "/candidates" },
    { label: "About", to: "/about" },
  ];

  return (
    <div className="flex min-h-screen flex-col justify-between bg-gray-50">
      {/* Reusable Navbar configured for Protected Dashboard */}
      <Navbar
        items={protectedItems}
        user={user}
        onLogout={() => logout()}
        isLoggingOut={isLoggingOut}
      />

      {isLoggingOut ? (
        <LoadingSpinner label="Logging out..." fullScreen={true} />
      ) : (
        /* Main Dashboard Content Area */
        <main className="mx-auto w-full max-w-7xl flex-1 px-4 py-6 sm:px-6 lg:px-8">
          {children}
        </main>
      )}
      {/* Reusable Footer */}
      <Footer items={protectedItems} />
    </div>
  );
};
