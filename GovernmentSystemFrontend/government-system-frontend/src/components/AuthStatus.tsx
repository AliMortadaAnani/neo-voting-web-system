import { type AppError } from "../api/api";

// components/AuthStatus.tsx
type AuthStatusProps = {
  isCheckingAuth: boolean;
  isAuthError: boolean;
  authError: AppError | null;
};

export const AuthStatus = ({
  isCheckingAuth,
  isAuthError,
  authError,
}: AuthStatusProps) => {
  if (isCheckingAuth) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-gray-50 dark:bg-zinc-950">
        <div className="flex flex-col items-center gap-3">
          <div className="h-8 w-8 animate-spin rounded-full border-4 border-blue-600 border-t-transparent" />
          <p className="text-sm font-medium text-gray-500 dark:text-zinc-400">
            Checking session...
          </p>
        </div>
      </div>
    );
  }

  if (isAuthError && authError?.status !== 401) {
    return (
      <div className="flex min-h-screen flex-col items-center justify-center gap-4 bg-gray-50 px-4 text-center dark:bg-zinc-950">
        <div className="rounded-2xl border border-red-200 bg-red-50 p-6 shadow-sm dark:border-red-900/50 dark:bg-red-950/20">
          <h2 className="text-lg font-semibold text-red-700 dark:text-red-400">
            {authError?.status === 0 ? "Server Unreachable" : "Server Error"}
          </h2>
          <p className="mt-1 text-sm text-red-600/80 dark:text-red-300/80">
            {authError?.status === 0
              ? "Unable to connect to the backend. Please check if the API is running."
              : "An unexpected error occurred while verifying your session."}
          </p>
          <button
            onClick={() => window.location.reload()}
            className="mt-4 inline-flex items-center rounded-lg bg-red-600 px-4 py-2 text-sm font-medium text-white transition hover:bg-red-700"
          >
            Retry Connection
          </button>
        </div>
      </div>
    );
  }

  return null;
};
