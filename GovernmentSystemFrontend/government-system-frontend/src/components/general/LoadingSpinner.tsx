type LoadingSpinnerProps = {
  label?: string;
  fullScreen?: boolean;
};

export const LoadingSpinner = ({
  label = "Loading...",
  fullScreen = true,
}: LoadingSpinnerProps) => {
  return (
    <div
      className={`flex w-full flex-col items-center justify-center gap-3 bg-white ${
        fullScreen ? "min-h-screen" : "py-12"
      }`}
    >
      {/* Spinner */}
      <div className="h-8 w-8 animate-spin rounded-full border-2 border-emerald-600 border-t-transparent" />

      {/* Static Text Label */}
      <p className="text-sm font-medium text-gray-600">{label}</p>
    </div>
  );
};
