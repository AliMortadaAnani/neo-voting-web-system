import { z } from "zod";

export const loginSchema = z.object({
  username: z
    .string()
    .min(1, "Username is required")
    .min(3, "Username must be at least 3 characters")
    .max(100, "Username must be at most 100 characters"),
  password: z
    .string()
    .min(1, "Password is required")
    .min(3, "Password must be at least 3 characters")
    .max(100, "Password must be at most 100 characters"),
});

