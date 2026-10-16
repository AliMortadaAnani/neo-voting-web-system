import { z } from "zod";
import type { Governorate, PagedResult } from "./citizen.types";
import { candidateFormSchema } from "../schemas/candidate.schemas";

export type CandidateResponseDTO = {
  id: number;
  nationalId: string;
  citizenId: number;
  nominationToken: string;
  hashedData: string;
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  governorate: Governorate;
  gender: "M" | "F" | string;
};

export type { PagedResult };

export type CandidateRequestDTO = z.infer<typeof candidateFormSchema>;
