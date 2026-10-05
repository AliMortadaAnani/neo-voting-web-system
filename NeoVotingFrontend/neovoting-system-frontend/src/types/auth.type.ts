export type Role = "Admin" | "Voter" | "Candidate";

export type AuthResponse = {
  accessToken: string;
  refreshToken?: string;
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

export type RegisterResponse = {
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

//type LoginRequest is exported from src/schemas/auth.schema.ts
//type RegisterRequest is exported from src/schemas/auth.schema.ts
