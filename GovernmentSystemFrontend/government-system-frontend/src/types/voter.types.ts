import { z } from "zod";
import type { Governorate, PagedResult } from "./citizen.types";
import { voterFormSchema } from "../schemas/voter.schemas";

export type VoterResponseDTO = {
  id: number;
  nationalId: string;
  citizenId: number;
  votingToken: string;
  hashedData: string;
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  governorate: Governorate;
  gender: "M" | "F" | string;
};

export type { PagedResult };

export type VoterRequestDTO = z.infer<typeof voterFormSchema>;
