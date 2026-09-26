import { api } from "../api/api";
import type { AuthResponse } from "../types/auth.types.ts";
import type { LoginCredentials } from "../schemas/auth.schemas";

export const login = async (
  credentials: LoginCredentials,
): Promise<AuthResponse> => {
  const response = await api.post<AuthResponse>("/auth/login", credentials);
  return response.data;
};

//it checks if the user is sending a valid cookie
export const getMe = async (): Promise<AuthResponse> => {
  const response = await api.get<AuthResponse>("/auth/me");
  return response.data;
};

export const logout = async (): Promise<string> => {
  const response = await api.post<string>("/auth/logout");
  return response.data;
};
