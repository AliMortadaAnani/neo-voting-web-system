import axios from "axios";

export type ProblemDetails = {
  type?: string; // code property in AppError
  title?: string; // deduced from status code
  status?: number; // status property in AppError
  detail?: string; // message property in AppError : Error
  instance?: string; // not used
  errors?: Record<string, string[]>; // errors property in AppError
  traceId?: string; // not used
};

export type AppError = Error & {
  code?: string; // type from ProblemDetails
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
  // Standard approach: passes through the full AxiosResponse
  (response) => response,

  (error) => {
    if (!error.response) {
      const message = "Network error: Server is offline or unreachable.";

      const offlineError: AppError = new Error(message);
      offlineError.code = "NETWORK_ERROR";
      offlineError.status = 0;
      offlineError.errors = {};

      return Promise.reject(offlineError);
    }

    const problem: ProblemDetails = error.response.data || {};
    const message =
      problem.detail || problem.type || "An unexpected error occurred.";

    const uiError: AppError = new Error(message);
    uiError.code = problem.type ?? error.response.statusText ?? "UNKNOWN_ERROR";
    uiError.status = problem.status ?? error.response.status ?? 500;
    uiError.errors = problem.errors ?? {};

    return Promise.reject(uiError);
  },
);
