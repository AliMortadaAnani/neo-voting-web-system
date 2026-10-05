import { api } from "../api/api";
import type { AuthResponse, RegisterResponse } from "../types/auth.type";
import type { LoginRequest, RegisterRequest } from "../schemas/auth.schema";

export const login = async (payload: LoginRequest): Promise<AuthResponse> => {
  return api.post("/Auth/login", payload);
};

export const logout = async (): Promise<void> => {
  return api.post("/Auth/logout");
};

export const registerVoter = async (
  payload: RegisterRequest,
): Promise<RegisterResponse> => {
  return api.post("/Auth/register-voter", payload);
};

export const registerCandidate = async (
  payload: RegisterRequest,
): Promise<RegisterResponse> => {
  return api.post("/Auth/register-candidate", payload);
};

// --- Role Verification Endpoints ---

export const testAdmin = async (): Promise<string> => {
  return api.get("/Auth/test-admin");
};

export const testVoter = async (): Promise<string> => {
  return api.get("/Auth/test-voter");
};

export const testCandidate = async (): Promise<string> => {
  return api.get("/Auth/test-candidate");
};
