import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { api } from "../services/api";
import { useNavigate } from "react-router-dom";
import {
  type AuthResponseDto,
  type LoginRequestDto,
} from "../types/auth.types";

export const useAuth = () => {
  const queryClient = useQueryClient();
  const navigate = useNavigate();

  const {
    data: authenticatedUserData,
    isLoading,
    isError,
  } = useQuery<AuthResponseDto>({
    queryKey: ["authUser"],
    queryFn: async () => {
      const { data } = await api.get("/auth/me");
      return data;
    },
    retry: false, // automatically retrying on failure (disabled now)
    staleTime: Infinity, //keep the user data fresh for the entire session
  });

  const loginMutation = useMutation({
    mutationFn: async (credentials: LoginRequestDto) => {
      const { data } = await api.post("/auth/login", credentials);
      return data;
    },
    onSuccess: (authenticatedUserData) => {
      queryClient.setQueryData(["authUser"], authenticatedUserData);
      navigate("/home", { replace: true });
    },
  });

  const logoutMutation = useMutation({
    mutationFn: async () => {
      await api.post("/auth/logout");
    },
    onSuccess: () => {
      queryClient.clear();
      navigate("/login", { replace: true });
    },
  });

  return {
    authenticatedUser: authenticatedUserData,
    isAuthenticated: !!authenticatedUserData && !isError,
    isLoading,

    login: loginMutation.mutateAsync,
    isLoggingIn: loginMutation.isPending,
    loginError: loginMutation.error,

    logout: logoutMutation.mutateAsync,
    isLoggingOut: logoutMutation.isPending,
    logoutError: logoutMutation.error,
  };
};
