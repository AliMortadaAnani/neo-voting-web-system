import React from "react";

export type SelectOption = {
  value: string | number;
  label: string;
};

type SelectFieldProps = React.ComponentProps<"select"> & {
  label: string;
  options: SelectOption[];
  placeholder?: string;
  error?: string;
};

export const SelectField = ({
  label,
  options,
  placeholder,
  error,
  id,
  className = "",
  disabled,
  ...props
}: SelectFieldProps) => {
  const selectId = id || props.name;

  return (
    <div className="w-full">
      {/* Accessible Label */}
      <label
        htmlFor={selectId}
        className="mb-1.5 block text-xs font-semibold tracking-wider text-stone-700 uppercase"
      >
        {label}
      </label>

      {/* Styled Native Select */}
      <select
        id={selectId}
        disabled={disabled}
        {...props}
        className={`w-full rounded-lg border px-3.5 py-2.5 text-sm text-stone-900 transition-colors focus:outline-none ${
          disabled ? "cursor-not-allowed bg-stone-50 opacity-75" : "bg-white"
        } ${
          error
            ? "border-red-500 focus:border-red-600 focus:ring-1 focus:ring-red-600"
            : "border-stone-300 focus:border-emerald-600 focus:ring-1 focus:ring-emerald-600"
        } ${className}`}
      >
        {placeholder && (
          <option value="" disabled>
            {placeholder}
          </option>
        )}
        {options.map((option) => (
          <option key={option.value} value={option.value}>
            {option.label}
          </option>
        ))}
      </select>

      {/* Red Validation Error */}
      {error && (
        <p className="mt-1.5 text-xs font-medium text-red-600">{error}</p>
      )}
    </div>
  );
};
