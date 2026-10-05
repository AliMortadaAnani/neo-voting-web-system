import { api } from "../api/api";
import type { LoginRequestDTO, AuthResponseDTO } from "../types/auth.types.ts";

export const login = async (dto: LoginRequestDTO): Promise<AuthResponseDTO> => {
  const response = await api.post<AuthResponseDTO>("/auth/login", dto);
  return response.data;
};

//it checks if the user is sending a valid cookie
export const getMe = async (): Promise<AuthResponseDTO> => {
  const response = await api.get<AuthResponseDTO>("/auth/me");
  return response.data;
};

export const logout = async (): Promise<string> => {
  const response = await api.post<string>("/auth/logout");
  return response.data;
};
