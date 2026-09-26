import type { PagedResult } from "./citizen.types";

export type VoterResponse = {
  id: number;
  nationalId: string;
  citizenId: number;
  votingToken: string;
  hashedData: string;
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  governorate: number;
  gender: "M" | "F" | string;
};

export type CreateVoterDTO = {
  nationalId: string;
};

export type UpdateVoterDTO = {
  nationalId: string;
};

export type { PagedResult };
