import { z } from "zod";

export const candidateProfileCreateSchema = z.object({
  nominationReasons: z
    .string()
    .min(3, "Minimum 3 characters")
    .max(4000, "Maximum 4000 characters"),
});
