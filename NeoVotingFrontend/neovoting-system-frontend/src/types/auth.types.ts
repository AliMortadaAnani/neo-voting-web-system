import { z } from "zod";
import {
  loginSchema,
  registerVoterOrCandidateSchema,
} from "../schemas/auth.schemas";

export type Role = "Admin" | "Voter" | "Candidate";

export type AuthResponseDTO = {
  accessToken: string;
  refreshToken: string;
  accessTokenExpiration: string;
  refreshTokenExpiration: string;

  // User Profile
  applicationUserId: number;
  accountId?: number | null;
  userName: string;
  firstName?: string | null;
  lastName?: string | null;
  governorate?: number | string | null;
  dateOfBirth?: string | null;
  gender?: string | null;
  role: Role;
};

export type RegisterVoterOrCandidateResponseDTO = {
  applicationUserId: number;
  accountId: number;
  userName: string;
  firstName: string;
  lastName: string;
  governorate: number | string;
  dateOfBirth: string;
  gender: string;
  role: Role;
  message?: string;
};

export type LoginRequestDTO = z.infer<typeof loginSchema>;
export type RegisterVoterOrCandidateRequestDTO = z.infer<
  typeof registerVoterOrCandidateSchema
>;
