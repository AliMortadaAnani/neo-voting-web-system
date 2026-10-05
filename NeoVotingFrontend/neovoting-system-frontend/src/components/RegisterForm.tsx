import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation } from "@tanstack/react-query";
import { useNavigate, Link } from "react-router-dom";
import toast from "react-hot-toast";
import { registerSchema, type RegisterRequest } from "../schemas/auth.schema";
import type { RegisterResponse } from "../types/auth.type";
import type { AppError } from "../api/api";

type RegisterFormProps = {
  roleTitle: "Voter" | "Candidate";
  tokenLabel: string;
  submitFn: (payload: RegisterRequest) => Promise<RegisterResponse>;
};

export const RegisterForm = ({
  roleTitle,
  tokenLabel,
  submitFn,
}: RegisterFormProps) => {
  const navigate = useNavigate();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<RegisterRequest>({
    resolver: zodResolver(registerSchema),
  });

  // Mutation handling network state
  const {
    mutateAsync,
    isPending,
    error: mutationError,
  } = useMutation<RegisterResponse, AppError, RegisterRequest>({
    mutationFn: submitFn,
    onSuccess: (data) => {
      toast.success(data.message || "Registration successful! Please sign in.");
      navigate("/login", { replace: true });
    },
    onError: (err) => {
      toast.error(err.message);
    },
  });

  const onSubmit = async (data: RegisterRequest) => {
    try {
      await mutateAsync(data);
    } catch {
      // Errors handled by onError and displayed below
    }
  };

  return (
    <div className="flex min-h-screen items-center justify-center bg-slate-100 p-4 py-12">
      <div className="w-full max-w-md rounded-2xl border border-slate-200 bg-white p-8 shadow-sm">
        <div className="mb-6 text-center">
          <h1 className="text-2xl font-bold tracking-tight text-slate-900">
            {roleTitle} Registration
          </h1>
          <p className="mt-1.5 text-sm text-slate-500">
            Enter your official details to create your {roleTitle.toLowerCase()}{" "}
            account
          </p>
        </div>

        {/* Server Error Alert Banner */}
        {mutationError && (
          <div className="mb-5 rounded-lg border border-red-200 bg-red-50 p-3 text-sm text-red-700">
            {mutationError.message}
          </div>
        )}

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          {/* Username */}
          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">
              Username
            </label>
            <input
              type="text"
              placeholder="e.g. john_doe"
              disabled={isPending}
              {...register("userName")}
              className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm text-slate-900 focus:border-slate-800 focus:outline-none disabled:bg-slate-100"
            />
            {errors.userName && (
              <p className="mt-1 text-xs text-red-600">
                {errors.userName.message}
              </p>
            )}
          </div>

          {/* National ID */}
          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">
              National ID
            </label>
            <input
              type="text"
              placeholder="Enter your National ID"
              disabled={isPending}
              {...register("nationalId")}
              className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm text-slate-900 focus:border-slate-800 focus:outline-none disabled:bg-slate-100"
            />
            {errors.nationalId && (
              <p className="mt-1 text-xs text-red-600">
                {errors.nationalId.message}
              </p>
            )}
          </div>

          {/* Dynamic Token Field */}
          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">
              {tokenLabel}
            </label>
            <input
              type="text"
              placeholder={`Enter your ${tokenLabel.toLowerCase()}`}
              disabled={isPending}
              {...register("votingOrNominationToken")}
              className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm text-slate-900 focus:border-slate-800 focus:outline-none disabled:bg-slate-100"
            />
            {errors.votingOrNominationToken && (
              <p className="mt-1 text-xs text-red-600">
                {errors.votingOrNominationToken.message}
              </p>
            )}
          </div>

          {/* New Password */}
          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">
              Password
            </label>
            <input
              type="password"
              placeholder="••••••••"
              disabled={isPending}
              {...register("newPassword")}
              className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm text-slate-900 focus:border-slate-800 focus:outline-none disabled:bg-slate-100"
            />
            {errors.newPassword && (
              <p className="mt-1 text-xs text-red-600">
                {errors.newPassword.message}
              </p>
            )}
          </div>

          {/* Confirm Password */}
          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">
              Confirm Password
            </label>
            <input
              type="password"
              placeholder="••••••••"
              disabled={isPending}
              {...register("confirmPassword")}
              className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm text-slate-900 focus:border-slate-800 focus:outline-none disabled:bg-slate-100"
            />
            {errors.confirmPassword && (
              <p className="mt-1 text-xs text-red-600">
                {errors.confirmPassword.message}
              </p>
            )}
          </div>

          {/* Submit Button */}
          <button
            type="submit"
            disabled={isPending}
            className="w-full rounded-lg bg-slate-900 py-2.5 text-sm font-semibold text-white transition-all hover:bg-slate-800 disabled:bg-slate-400"
          >
            {isPending ? "Creating Account..." : `Register as ${roleTitle}`}
          </button>
        </form>

        {/* Back to Login Link */}
        <div className="mt-6 border-t border-slate-100 pt-5 text-center text-xs text-slate-500">
          Already registered?{" "}
          <Link
            to="/login"
            className="font-semibold text-slate-800 hover:underline"
          >
            Sign in here
          </Link>
        </div>
      </div>
    </div>
  );
};
