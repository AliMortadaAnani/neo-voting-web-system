import { api } from "../api/api";
import type { LoginRequestDTO, AuthResponseDTO } from "../types/auth.types.ts";

export const login = async (dto: LoginRequestDTO): Promise<AuthResponseDTO> => {
  console.log("[Auth] Login started:", dto);
  const response = await api.post<AuthResponseDTO>("/auth/login", dto);
  console.log("[Auth] Login completed, response:", response.data);
  return response.data;
};

//it checks if the user is sending a valid cookie
export const getMe = async (): Promise<AuthResponseDTO> => {
  console.log("[Auth] Checking current user (from cookie)");
  const response = await api.get<AuthResponseDTO>("/auth/me");
  console.log("[Auth] Current user check completed, response:", response.data);
  return response.data;
};

export const logout = async (): Promise<string> => {
  console.log("[Auth] Logout started");
  const response = await api.post<string>("/auth/logout");
  console.log("[Auth] Logout completed, response:", response.data);
  return response.data;
};
