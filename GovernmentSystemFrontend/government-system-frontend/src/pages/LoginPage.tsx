import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useNavigate } from "react-router-dom";
import { loginSchema, type LoginCredentials } from "../schemas/auth.schemas";
import { useAuth } from "../hooks/useAuth";

export const LoginPage = () => {
  const navigate = useNavigate();
  const { login, isLoggingIn } = useAuth();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginCredentials>({
    resolver: zodResolver(loginSchema),
  });

  const onSubmit = (data: LoginCredentials) => {
    login(data, {
      onSuccess: () => {
        navigate("/citizens", { replace: true });
      },
    });
  };

  return (
    <div className="flex min-h-screen items-center justify-center bg-slate-100 p-4">
      <div className="w-full max-w-md rounded-xl bg-white p-6 shadow-md">
        <h1 className="mb-6 text-center text-2xl font-bold text-slate-800">
          Admin Login
        </h1>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          {/* Username Input */}
          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">
              Username
            </label>
            <input
              type="text"
              placeholder="Enter username"
              {...register("username")}
              className="w-full rounded-lg border border-slate-300 p-2.5 text-sm focus:border-slate-800 focus:outline-none"
            />
            {errors.username && (
              <p className="mt-1 text-xs text-red-600">
                {errors.username.message}
              </p>
            )}
          </div>

          {/* Password Input */}
          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">
              Password
            </label>
            <input
              type="password"
              placeholder="••••••••"
              {...register("password")}
              className="w-full rounded-lg border border-slate-300 p-2.5 text-sm focus:border-slate-800 focus:outline-none"
            />
            {errors.password && (
              <p className="mt-1 text-xs text-red-600">
                {errors.password.message}
              </p>
            )}
          </div>

          {/* Submit Button */}
          <button
            type="submit"
            disabled={isLoggingIn}
            className="w-full rounded-lg bg-slate-900 py-2.5 text-sm font-medium text-white hover:bg-slate-800 disabled:bg-slate-400"
          >
            {isLoggingIn ? "Authenticating..." : "Sign In"}
          </button>
        </form>
      </div>
    </div>
  );
};
