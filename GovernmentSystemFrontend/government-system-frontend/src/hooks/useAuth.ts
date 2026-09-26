import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import toast from "react-hot-toast";
import { getMe, login, logout } from "../services/auth.services";
import type { AppError } from "../api/api";
import type { AuthResponse } from "../types/auth.types.ts";
import type { LoginCredentials } from "../schemas/auth.schemas";

export const useAuthCheck = () => {
  const {
    data: user,
    isLoading: isCheckingAuth,
    isError: isAuthError,
    error: authError,
  } = useQuery<AuthResponse, AppError>({
    queryKey: ["auth", "me"],
    queryFn: getMe,
    retry: false, // Don't retry on 401
    staleTime: 1000 * 60 * 10, // 10 minutes
  });

  return {
    user,
    isAuthenticated: !!user?.isSuccess,
    isCheckingAuth,
    isAuthError,
    authError,
  };
};

export const useAuth = () => {
  const queryClient = useQueryClient();

  const loginMutation = useMutation<AuthResponse, AppError, LoginCredentials>({
    mutationFn: login,
    onSuccess: (data) => {
      queryClient.setQueryData(["auth", "me"], data);
      toast.success(data.message || "Welcome!");
    },
    onError: (error: AppError) => {
      toast.error(error.message || "Invalid credentials");
    },
  });

  const logoutMutation = useMutation<string, AppError, void>({
    mutationFn: logout,
    onSuccess: (data) => {
      queryClient.setQueryData(["auth", "me"], null);
      queryClient.removeQueries(); // Clears all sensitive cached data safely
      toast.success(data || "Logged out successfully");
    },
    onError: (error: AppError) => {
      toast.error(error.message || "Failed to log out");
    },
  });

  return {
    // Both mutate (safe) and mutateAsync (promise) available:
    login: loginMutation.mutate,
    loginAsync: loginMutation.mutateAsync,
    isLoggingIn: loginMutation.isPending,
    isLoginError: loginMutation.isError,
    loginError: loginMutation.error,

    logout: logoutMutation.mutate,
    logoutAsync: logoutMutation.mutateAsync,
    isLoggingOut: logoutMutation.isPending,
    isLogoutError: logoutMutation.isError,
    logoutError: logoutMutation.error,
  };
};
