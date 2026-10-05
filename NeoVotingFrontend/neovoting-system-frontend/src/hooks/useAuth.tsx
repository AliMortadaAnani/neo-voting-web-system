import {
  createContext,
  useContext,
  useState,
  useEffect,
  type ReactNode,
} from "react";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import toast from "react-hot-toast";

import {
  getAccessToken,
  setAccessToken,
  clearAccessToken,
} from "../utils/token";

import { decodeUser, type UserClaims } from "../utils/jwt";
import { login, logout } from "../services/auth.services";
import type { AuthResponse } from "../types/auth.type";
import type { LoginRequest } from "../schemas/auth.schema";
import type { AppError } from "../api/api";

// --- 1. Context: Strictly stores claims ---

type AuthContextType = {
  user: UserClaims | null;
  setUser: (user: UserClaims | null) => void;
  isCheckingAuth: boolean;
};

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<UserClaims | null>(null);
  const [isCheckingAuth, setIsCheckingAuth] = useState(true);

  // Read and decode claims from localStorage on startup
  useEffect(() => {
    const token = getAccessToken();

    if (token) {
      const claims = decodeUser(token);
      // Valid session strictly requires a role
      if (claims?.role) {
        setUser(claims);
      } else {
        clearAccessToken();
        setUser(null);
      }
    }

    setIsCheckingAuth(false);
  }, []);

  return (
    <AuthContext value={{ user, setUser, isCheckingAuth }}>
      {children}
    </AuthContext>
  );
};

// --- 2. Custom Hook: Combines Claims + TanStack Mutations ---

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }

  const { user, setUser, isCheckingAuth } = context;
  const queryClient = useQueryClient();

  // Login Mutation: TanStack Query handles network, loading, and error states
  const loginMutation = useMutation<AuthResponse, AppError, LoginRequest>({
    mutationFn: login,
    onSuccess: (data) => {
      setAccessToken(data.accessToken);

      const claims = decodeUser(data.accessToken);
      if (!claims?.role) {
        throw new Error("Invalid token claims received from server.");
      }

      setUser(claims);
      toast.success(`Welcome, ${claims.username}!`);
    },
    onError: (error) => {
      toast.error(error.message);
    },
  });

  // Logout Mutation: Clears storage, wipes memory/cache, resets claims
  const logoutMutation = useMutation<void, AppError, void>({
    mutationFn: logout,
    onSettled: () => {
      clearAccessToken();
      setUser(null);
      queryClient.clear();
      toast.success("Logged out successfully");
    },
  });

  return {
    // Claims & Identity
    user,
    role: user?.role ?? null,
    // Strictly authenticated only if a valid role exists
    isAuthenticated: Boolean(user?.role),
    isCheckingAuth,

    // Login Mutation
    login: loginMutation.mutateAsync,
    isLoggingIn: loginMutation.isPending,
    loginError: loginMutation.error,

    // Logout Mutation
    logout: logoutMutation.mutateAsync,
    isLoggingOut: logoutMutation.isPending,
  };
};
