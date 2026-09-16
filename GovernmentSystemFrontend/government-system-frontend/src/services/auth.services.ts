import { api, type AppError } from "../api/api";
import type { AuthResponse } from "../types/auth.type";
import type { LoginCredentials } from "../schemas/auth.schema";
import { toast } from "react-hot-toast/headless";

export const login = async (
  credentials: LoginCredentials,
): Promise<AuthResponse> => {
  return api.post("/auth/login", credentials);
};

// Conceptual plan for auth.services.ts:
export const getMe = async (): Promise<AuthResponse> => {
  try {
    return await api.get("/auth/me");
  } catch (error) {
    const appErr = error as AppError;
    // If it's a 401, treat it as a valid guest state:
    if (appErr.status === 401) {
      toast.error("You are not logged in. Please log in to continue.");
      return { isSuccess: false, message: "Guest", username: "", role: "" };
    }
    if (appErr.status === 0) {
      return { isSuccess: false, message: "Offline", username: "", role: "" };
    }
    // Any other error (500, server offline) is re-thrown
    throw appErr;
  }
};

export const logout = async (): Promise<string> => {
  return api.post("/auth/logout");
};
