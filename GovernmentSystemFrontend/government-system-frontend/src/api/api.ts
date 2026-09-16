import axios from "axios";

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

export const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || "http://localhost:5000/api",
  withCredentials: true,
  headers: {
    "Content-Type": "application/json",
  },
});

api.interceptors.response.use(
  (response) => response.data,
  (error) => {
    if (!error.response) {
      const offlineError: AppError = new Error(
        "Network error: Server is offline or unreachable.",
      );
      offlineError.status = 0;

      return Promise.reject(offlineError);
    }

    const problem: ProblemDetails = error.response.data || {};
    const message =
      problem.detail || problem.type || "An unexpected error occurred.";

    const uiError: AppError = new Error(message);
    uiError.code = problem.type;
    uiError.status = problem.status ?? error.response.status;
    uiError.errors = problem.errors;

    return Promise.reject(uiError);
  },
);
