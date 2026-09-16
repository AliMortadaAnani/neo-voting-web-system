import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import toast from "react-hot-toast";
import { getMe, login, logout } from "../services/auth.services";
import type { AppError } from "../api/api";
import type { AuthResponse } from "../types/auth.type";
import type { LoginCredentials } from "../schemas/auth.schema";

export const useAuth = () => {
  const queryClient = useQueryClient();

  // 1. Session check: Reads GET /auth/me to verify the cookie
  const { data: user, isLoading: isCheckingAuth } = useQuery<
    AuthResponse,
    AppError
  >({
    queryKey: ["auth", "me"],
    queryFn: getMe,
    retry: false, // Never retry on failure
    staleTime: 1000 * 60 * 60, // Cache session for 1 hour before re-checking
  });

  // 2. Login mutation: Submits credentials, updates session cache, and toasts result
  const loginMutation = useMutation<AuthResponse, AppError, LoginCredentials>({
    mutationFn: login,
    onSuccess: (data) => {
      // Instantly inject user into cache so the app knows we're logged in
      queryClient.setQueryData(["auth", "me"], data);
      toast.success(data.message || "Welcome back!");
    },
    onError: (error) => {
      toast.error(error.message);
    },
  });

  // 3. Logout mutation: Clears cookie on server, wipes client cache, and toasts result
  const logoutMutation = useMutation<string, AppError, void>({
    mutationFn: logout,
    onSuccess: (message) => {
      // Clear entire TanStack cache so no citizen/voter data lingers in memory
      queryClient.setQueryData(["auth", "me"], null);
      queryClient.clear();
      toast.success(message || "Logged out successfully");
    },
    onError: (error) => {
      toast.error(error.message);
    },
  });

  return {
    user,
    isAuthenticated: Boolean(user?.isSuccess),
    isCheckingAuth,
    // Mutations & states
    //login: loginMutation.mutate,
    loginAsync: loginMutation.mutateAsync,
    isLoggingIn: loginMutation.isPending,
    loginError: loginMutation.error,
    //logout: logoutMutation.mutate,
    logoutAsync: logoutMutation.mutateAsync,
    isLoggingOut: logoutMutation.isPending,
    logoutError: logoutMutation.error,
  };
};
