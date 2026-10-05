import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useNavigate, Link } from "react-router-dom";
import { loginSchema, type LoginRequest } from "../schemas/auth.schema";
import { useAuth } from "../hooks/useAuth";
import { ROLE_HOME } from "../utils/jwt";

export const LoginPage = () => {
  const navigate = useNavigate();
  const { login, isLoggingIn, loginError } = useAuth();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginRequest>({
    resolver: zodResolver(loginSchema),
  });

  const onSubmit = async (data: LoginRequest) => {
    try {
      // 1. Submit login and decode claims
      const claims = await login(data);

      // 2. Dynamically redirect based on role (Admin -> /admin, Voter -> /voter, etc.)
      navigate(ROLE_HOME[claims.role], { replace: true });
    } catch {
      // Error toast fires automatically via useAuth
      // loginError banner updates automatically below
    }
  };

  return (
    <div className="flex min-h-screen items-center justify-center bg-slate-100 p-4">
      <div className="w-full max-w-md rounded-2xl bg-white p-8 shadow-sm border border-slate-200">
        <div className="mb-6 text-center">
          <h1 className="text-2xl font-bold tracking-tight text-slate-900">
            Voting Portal Login
          </h1>
          <p className="mt-1.5 text-sm text-slate-500">
            Sign in to access your voting or administration panel
          </p>
        </div>

        {/* Server Error Banner */}
        {loginError && (
          <div className="mb-5 rounded-lg border border-red-200 bg-red-50 p-3 text-sm text-red-700">
            {loginError.message}
          </div>
        )}

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          {/* Username Field */}
          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">
              Username
            </label>
            <input
              type="text"
              placeholder="Enter your username"
              disabled={isLoggingIn}
              {...register("userName")}
              className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm text-slate-900 focus:border-slate-800 focus:outline-none disabled:bg-slate-100"
            />
            {errors.userName && (
              <p className="mt-1 text-xs text-red-600">
                {errors.userName.message}
              </p>
            )}
          </div>

          {/* Password Field */}
          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">
              Password
            </label>
            <input
              type="password"
              placeholder="••••••••"
              disabled={isLoggingIn}
              {...register("password")}
              className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm text-slate-900 focus:border-slate-800 focus:outline-none disabled:bg-slate-100"
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
            className="w-full rounded-lg bg-slate-900 py-2.5 text-sm font-semibold text-white transition-all hover:bg-slate-800 disabled:bg-slate-400"
          >
            {isLoggingIn ? "Authenticating..." : "Sign In"}
          </button>
        </form>

        {/* Registration Quick Links */}
        <div className="mt-6 border-t border-slate-100 pt-5 text-center text-xs text-slate-500">
          <p>Don't have an account yet?</p>
          <div className="mt-2 flex items-center justify-center gap-4 font-semibold text-slate-800">
            <Link to="/register/voter" className="hover:underline">
              Register as Voter
            </Link>
            <span>•</span>
            <Link to="/register/candidate" className="hover:underline">
              Register as Candidate
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
};

export default LoginPage;
