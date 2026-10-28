// layouts/ProtectedLayout.tsx
import React from "react";
import { Navbar, type NavItem } from "../../components/general/Navbar";
import { Footer } from "../../components/general/Footer";
import { useAuth } from "../../hooks/useAuth";
import { LoadingSpinner } from "../../components/general/LoadingSpinner";
import { Button } from "./Button";
import { useGoBack } from "../../hooks/useUtils";
import type { AuthResponseDTO } from "../../types/auth.types";
type ProtectedLayoutProps = {
  children: React.ReactNode;
  user: AuthResponseDTO | undefined; // user can be null if not authenticated
};

export const ProtectedLayout = ({ children, user }: ProtectedLayoutProps) => {
  const { logout, isLoggingOut } = useAuth();

  const handleBack = useGoBack();
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
          {/* Sticky Return Button at Top  */}
          <div className="sticky top-20 z-10 flex justify-end">
            <Button
              label="← Return"
              onClick={handleBack}
              color="green"
              size="sm"
            />
          </div>
          {children}
        </main>
      )}
      {/* Reusable Footer */}
      <Footer items={protectedItems} />
    </div>
  );
};
