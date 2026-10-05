import { z } from "zod";
import type { PagedResult } from "./citizen.types";
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
  governorate: number;
  gender: "M" | "F" | string;
};

export type { PagedResult };

export type CandidateRequestDTO = z.infer<typeof candidateFormSchema>;
