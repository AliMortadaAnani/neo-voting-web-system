import type { PagedResult } from "./citizen.types";

export type CandidateResponse = {
  id: number;
  nationalId: string;
  citizenId: number;
  nominationToken: string;
  hashedData: string;
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  governorate: number;
  gender: "M" | "F" | string;
};

export type CreateCandidateDTO = {
  nationalId: string;
};

export type UpdateCandidateDTO = {
  nationalId: string;
};

export type { PagedResult };
