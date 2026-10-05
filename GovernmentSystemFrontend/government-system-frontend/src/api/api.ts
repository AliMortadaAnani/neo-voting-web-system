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
  errors?: Record<string, string[]>; //from ASP.NET or FluentValidation
};

export const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || "http://localhost:5000/api",
  withCredentials: true,
  headers: {
    "Content-Type": "application/json", // specifies that the request body will be in JSON format
    //if we want to send images or files, we can use "multipart/form-data" instead of "application/json"...
  },
});

api.interceptors.response.use(
  // Standard approach: passes through the full AxiosResponse
  (response) => response,

  (error) => {
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
