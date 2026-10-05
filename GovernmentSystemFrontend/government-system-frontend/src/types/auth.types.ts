import { z } from "zod";
import { loginSchema } from "../schemas/auth.schemas";

export type AuthResponseDTO = {
  isSuccess: boolean;
  message: string;
  username: string;
  role: string;
};

export type LoginRequestDTO = z.infer<typeof loginSchema>;
