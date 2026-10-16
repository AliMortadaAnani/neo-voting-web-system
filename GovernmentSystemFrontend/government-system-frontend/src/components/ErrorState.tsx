// components/ErrorState.tsx
import type { AppError } from "../api/api";
import { Button } from "./Button";

type ErrorStateProps = {
  error?: AppError | null;
  fullScreen?: boolean;
};

export const ErrorState = ({ error, fullScreen = true }: ErrorStateProps) => {
  const isOffline = error?.status === 0;
  const title = isOffline ? "Server Unreachable" : "Server Error";
  const message = isOffline
    ? "Unable to connect to the backend server. Please make sure the API is active."
    : error?.message ||
      "An unexpected error occurred while communicating with the server.";

  const handleRetry = () => window.location.reload();

  return (
    <div
      className={`flex w-full items-center justify-center bg-gray-50 px-4 ${
        fullScreen ? "min-h-screen" : "py-12"
      }`}
    >
      <div className="w-full max-w-md rounded-xl border border-stone-300 bg-white p-6 text-center shadow-sm">
        {/* here no need to make the div flex since h2, p are block elements */}
        {/* Red Alert Header */}
        <h2 className="text-lg font-bold text-red-600">{title}</h2>

        {/* Informative Explanation */}
        <p className="mt-2 text-sm text-gray-600">{message}</p>

        {/* Our Extracted Reusable Button */}
        <div className="mt-6">
          <Button
            label="Try Again"
            onClick={handleRetry}
            color="green"
            size="md"
          />
        </div>
      </div>
    </div>
  );
};
