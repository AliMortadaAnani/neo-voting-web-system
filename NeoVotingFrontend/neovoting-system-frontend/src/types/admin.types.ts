// types/admin.type.ts
import { z } from "zod";
import {
  electionCreateSchema,
  pollCreateSchema,
} from "../schemas/admin.schemas";

export type ElectionResponseDTO = {
  id: number;
  name: string;
  nominationStartDate: string;
  nominationEndDate: string;
  votingStartDate: string;
  votingEndDate: string;
  status: string;
};

export type SystemAuditLogResponseDTO = {
  id: number;
  timestampUTC: string;
  actionType: string;
  details?: string;
  adminId: number;
  username: string;
};

export type ElectionCreateRequestDTO = z.infer<typeof electionCreateSchema>;
export type PollCreateRequestDTO = z.infer<typeof pollCreateSchema>;
