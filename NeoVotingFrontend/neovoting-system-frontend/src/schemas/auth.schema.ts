import { z } from "zod";

// --- Login Schema ---
export const loginSchema = z.object({
  userName: z
    .string()
    .min(1, "Username is required")
    .min(3, "Username must be at least 3 characters")
    .max(100, "Username cannot exceed 100 characters"),
  password: z
    .string()
    .min(1, "Password is required")
    .min(3, "Password must be at least 3 characters")
    .max(100, "Password cannot exceed 100 characters"),
});

export type LoginRequest = z.infer<typeof loginSchema>;

// --- Unified Registration Schema (Voter & Candidate) ---
export const registerSchema = z
  .object({
    userName: z
      .string()
      .min(1, "Username is required")
      .min(3, "Username must be at least 3 characters")
      .max(100, "Username cannot exceed 100 characters"),
    newPassword: z
      .string()
      .min(1, "Password is required")
      .min(3, "Password must be at least 3 characters")
      .max(100, "Password cannot exceed 100 characters"),
    confirmPassword: z.string().min(1, "Please confirm your password"),
    nationalId: z
      .string()
      .min(1, "National ID is required")
      .max(100, "National ID cannot exceed 100 characters"),
    votingOrNominationToken: z
      .string()
      .min(1, "Token is required")
      .max(100, "Token cannot exceed 100 characters"),
  })
  .refine((data) => data.newPassword === data.confirmPassword, {
    message: "NewPassword and ConfirmPassword must match",
    path: ["confirmPassword"], // Attaches error directly to the confirmPassword field
  });

export type RegisterRequest = z.infer<typeof registerSchema>;
