import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import toast from "react-hot-toast";
import { getMe, login, logout } from "../services/auth.services";
import type { AppError } from "../api/api";
import type { AuthResponseDTO, LoginRequestDTO } from "../types/auth.types.ts";

export const useAuthCheck = () => {
  const {
    data: user,
    isLoading: isCheckingAuth,
    isError: isAuthError,
    error: authError,
  } = useQuery<AuthResponseDTO, AppError>({
    queryKey: ["auth", "me"],
    queryFn: getMe,
    retry: false, // Don't retry on 401
    staleTime: 1000 * 60 * 15, // 15 minutes
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

  const loginMutation = useMutation<AuthResponseDTO, AppError, LoginRequestDTO>(
    {
      mutationFn: login,
      onSuccess: (data) => {
        queryClient.setQueryData(["auth", "me"], data);
        toast.success(data.message || "Welcome!");
      },
      onError: (error: AppError) => {
        toast.error(error.message || "Invalid credentials");
      },
    },
  );

  const logoutMutation = useMutation<string, AppError, void>({
    mutationFn: logout,
    onSuccess: (data) => {
      queryClient.setQueryData(["auth", "me"], null); // since protected routes are subscribed to this query, they will automatically redirect to login page when the user logs out
      queryClient.clear(); // Clears all sensitive cached data safely
      toast.success(data || "Logged out successfully");
    },
    onError: (error: AppError) => {
      toast.error(error.message || "Failed to log out");
    },
  });

  return {
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
