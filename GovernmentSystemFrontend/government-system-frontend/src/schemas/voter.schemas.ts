import { z } from "zod";

export const voterFormSchema = z.object({
  nationalId: z
    .string()
    .trim()
    .min(1, "National ID is required")
    .max(100, "Maximum 100 characters"),
});

export type VoterFormValues = z.infer<typeof voterFormSchema>;
