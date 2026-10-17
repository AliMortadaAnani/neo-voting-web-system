// components/FormCard.tsx
import React from "react";

type FormCardMaxWidth = "md" | "lg" | "xl" | "2xl";

type FormCardProps = {
  title: string;
  subtitle?: string;
  maxWidth?: FormCardMaxWidth;
  children: React.ReactNode;
};

const maxWidthStyles: Record<FormCardMaxWidth, string> = {
  md: "max-w-md",
  lg: "max-w-lg",
  xl: "max-w-xl",
  "2xl": "max-w-2xl",
};

export const FormCard = ({
  title,
  subtitle,
  maxWidth = "md",
  children,
}: FormCardProps) => {
  return (
    <div
      className={`w-full ${maxWidthStyles[maxWidth]} mx-auto rounded-xl border border-stone-200 bg-white p-6 shadow-sm sm:p-8`}
    >
      {/* Card Header */}
      <div className="mb-6 text-center">
        <h1 className="text-2xl font-bold tracking-tight text-red-600">
          {title}
        </h1>
        {subtitle && (
          <p className="mt-1 text-xs font-semibold tracking-wider text-emerald-700 uppercase">
            {subtitle}
          </p>
        )}
      </div>

      {/* Form Content */}
      {children}
    </div>
  );
};
