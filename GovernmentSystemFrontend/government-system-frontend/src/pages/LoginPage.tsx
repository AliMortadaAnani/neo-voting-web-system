// pages/LoginPage.tsx
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { loginSchema } from "../schemas/auth.schemas";
import type { LoginRequestDTO } from "../types/auth.types";
import { useAuth } from "../hooks/useAuth";
import { Button } from "../components/Button";
import { InputField } from "../components/InputField";
import { FormCard } from "../components/FormCard";

export const LoginPage = () => {
  const { login, isLoggingIn, loginResponse } = useAuth();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginRequestDTO>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      username: "",
      password: "",
    },
  });

  const onSubmit = async (data: LoginRequestDTO) => {
    login(data);
  };

  return (
    <FormCard
      title="Admin Portal"
      subtitle="Republic of Lebanon — Official Authentication"
      maxWidth="md"
    >
      <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-4">
        {/* Username Field */}
        <InputField
          label="Username"
          type="text"
          autoComplete="username"
          placeholder="Enter username"
          disabled={isLoggingIn}
          error={errors.username?.message}
          {...register("username")}
        />

        {/* Password Field */}
        <InputField
          label="Password"
          type="password"
          autoComplete="current-password"
          placeholder="Enter password"
          disabled={isLoggingIn}
          error={errors.password?.message}
          {...register("password")}
        />

        {/* Submit Action */}
        <div className="mt-2 flex flex-col">
          <Button
            label={isLoggingIn ? "Signing In..." : "Sign In"}
            color="green"
            size="md"
            isLoading={isLoggingIn}
          />
        </div>
        {loginResponse && (
          <p className="text-sm font-medium text-emerald-600">
            {loginResponse.role}
          </p>
        )}
      </form>
    </FormCard>
  );
};
