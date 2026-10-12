import { z } from "zod";

import type { candidateProfileCreateSchema } from "../schemas/candidates.schemas";

export type CandidateProfileCreateRequestDTO = z.infer<
  typeof candidateProfileCreateSchema
>;
