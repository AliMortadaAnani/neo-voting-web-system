import axios from "axios";

export interface ProblemDetails {
  type?: string;
  title: string;
  status: number;
  detail: string;
  instance?: string;
  // Added dictionary to catch FluentValidation / ModelState errors
  errors?: Record<string, string[]>;
}

export const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || "http://localhost:3000/api",
  withCredentials: true,
  headers: {
    "Content-Type": "application/json",
  },
});

api.interceptors.response.use(
  (response) => response.data,
  (error) => {
    if (!error.response) {
      return Promise.reject(
        new Error("Network error: Server is offline or unreachable."),
      );
    }

    const problem: ProblemDetails = error.response.data;

    const uiError = new Error(
      problem.detail || "An unexpected error occurred.",
    ) as Error & {
      code?: string;
      status?: number;
      errors?: Record<string, string[]>;
    };

    uiError.code = problem.type;
    uiError.status = problem.status;
    uiError.errors = problem.errors; // Expose field errors to the UI component/form

    return Promise.reject(uiError);
  },
);
