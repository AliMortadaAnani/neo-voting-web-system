type ButtonColor = "green" | "red" | "stone";
type ButtonSize = "sm" | "md" | "lg";

type ButtonProps = {
  label: string;
  onClick?: () => void;
  color?: ButtonColor;
  size?: ButtonSize;
  isLoading?: boolean;
};

// Maps color options directly to our theme classes
const colorStyles: Record<ButtonColor, string> = {
  green: "bg-emerald-600 hover:bg-emerald-700 text-white",
  red: "bg-red-600 hover:bg-red-700 text-white",
  stone: "bg-stone-100 hover:bg-stone-200 text-stone-800",
};

// Maps size options to standard padding & text sizing
const sizeStyles: Record<ButtonSize, string> = {
  sm: "px-3 py-1.5 text-xs",
  md: "px-5 py-2.5 text-sm",
  lg: "px-6 py-3 text-base",
};

export const Button = ({
  label,
  onClick,
  color = "green",
  size = "md",
  isLoading = false,
}: ButtonProps) => {
  return (
    <button
      onClick={onClick}
      disabled={isLoading}
      className={`inline-flex cursor-pointer items-center justify-center rounded-lg font-semibold disabled:cursor-not-allowed disabled:opacity-50 ${colorStyles[color]} ${sizeStyles[size]} `}
    >
      {label}
    </button>
  );
};
