import axios, { type InternalAxiosRequestConfig } from "axios";
import {
  clearAccessToken,
  getAccessToken,
  setAccessToken,
} from "../utils/token";

export type ProblemDetails = {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  errors?: Record<string, string[]>;
  traceId?: string;
};

export type AppError = Error & {
  code?: string;
  status?: number;
  errors?: Record<string, string[]>;
};

// Base Axios instance
export const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || "http://localhost:7057/api",
  withCredentials: true, // Globally allow cookies (accepts Set-Cookie & sends cookies)
  headers: {
    "Content-Type": "application/json",
  },
});

// 1. Request Interceptor: Attach Bearer token to all outgoing requests if present
api.interceptors.request.use((config: InternalAxiosRequestConfig) => {
  const token = getAccessToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

api.interceptors.response.use(
  // Standard approach: passes through the full AxiosResponse
  (response) => response,

  async (error) => {
    // no response from the server => a network error or server is offline
    //caused by the server being offline, CORS, ip whitelisting ...
    if (!error.response) {
      const message = "Network error: Server is offline or unreachable.";

      const offlineError: AppError = new Error(message);
      offlineError.code = "NETWORK_ERROR";
      offlineError.status = 0;
      offlineError.errors = {};

      return Promise.reject(offlineError);
    }

    const originalRequest = error.config as InternalAxiosRequestConfig & {
      _retry?: boolean;
    };

    const status = error.response.status;
    const url = (originalRequest.url || "").toLowerCase();

    // The ONLY two endpoints that must never trigger a refresh:
    // 1. /Auth/login (bad credentials is not an expired session)
    // 2. /Auth/refresh-token (prevents infinite refresh loops)
    const isAuthEndpoint =
      url.includes("/auth/login") ||
      url.includes("/auth/refresh-token") ||
      url.includes("/auth/register");

    // B. If 401 on any protected action and we haven't retried yet:
    if (status === 401 && !isAuthEndpoint && !originalRequest._retry) {
      originalRequest._retry = true; // Stamp: "Tried once"

      try {
        const expiredToken = getAccessToken();

        // Call /Auth/refresh-token with expired token header + refresh cookie
        const refreshResponse = await axios.post<{ accessToken: string }>(
          `${api.defaults.baseURL}/Auth/refresh-token`,
          {},
          {
            withCredentials: true,
            headers: {
              Authorization: expiredToken ? `Bearer ${expiredToken}` : "",
            },
          },
        );

        const newAccessToken = refreshResponse.data.accessToken;

        // Save new token in localStorage & update failed request header
        setAccessToken(newAccessToken);
        originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;

        // Re-execute original request seamlessly
        return api(originalRequest);
      } catch (refreshError) {
        // Refresh token expired or invalid -> clear storage and kick to /login
        clearAccessToken();
        window.location.href = "/login";
        return Promise.reject(refreshError);
      }
    }

    const problem: ProblemDetails = error.response.data || {};

    const fieldErrors = problem.errors
      ? Object.values(problem.errors).flat().join(" ")
      : ""; //from ASP.NET or FluentValidation, we can have multiple errors for a single field, so we flatten them into a single string
    //In normal cases we should not obtain this error message expect if our handling was bypassed
    // zod and typescript should handle this before sending the request to the backend

    const message =
      fieldErrors ||
      problem.detail ||
      problem.type ||
      "An unexpected error occurred.";

    const uiError: AppError = new Error(message);
    uiError.code = problem.type ?? error.response.statusText ?? "UNKNOWN_ERROR";
    uiError.status = problem.status ?? error.response.status ?? 500;
    uiError.errors = problem.errors ?? {};

    return Promise.reject(uiError);
  },
);
