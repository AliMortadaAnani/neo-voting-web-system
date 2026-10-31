import React from "react";

type DateFieldProps = Omit<React.ComponentProps<"input">, "type"> & {
  label: string;
  error?: string;
  showTime?: boolean;
};

export const DateField = ({
  label,
  error,
  showTime = false,
  id,
  className = "",
  disabled,
  ...props
}: DateFieldProps) => {
  const inputId = id || props.name;

  return (
    <div className="w-full">
      {/* Accessible Label */}
      <label
        htmlFor={inputId}
        className="mb-1.5 block text-xs font-semibold tracking-wider text-stone-700 uppercase"
      >
        {label}
      </label>

      {/* Styled Native Date / Datetime Input */}
      <input
        id={inputId}
        type={showTime ? "datetime-local" : "date"}
        disabled={disabled}
        {...props}
        className={`w-full rounded-lg border px-3.5 py-2.5 text-sm text-stone-900 transition-colors focus:outline-none ${
          disabled ? "cursor-not-allowed bg-stone-50 opacity-75" : "bg-white"
        } ${
          error
            ? "border-red-500 focus:border-red-600 focus:ring-1 focus:ring-red-600"
            : "border-stone-300 focus:border-emerald-600 focus:ring-1 focus:ring-emerald-600"
        } ${className}`}
      />

      {/* Red Validation Error */}
      {error && (
        <p className="mt-1.5 text-xs font-medium text-red-600">{error}</p>
      )}
    </div>
  );
};
