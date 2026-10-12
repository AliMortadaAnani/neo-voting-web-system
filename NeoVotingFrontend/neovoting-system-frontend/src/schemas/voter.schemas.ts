// schemas/voter.schema.ts
import { z } from "zod";

export const voterCastElectionSchema = z.object({
  selectedCandidateProfileId: z.number({
    message: "SelectedCandidateProfileId is required.",
  }),
});

export const voterCastPollSchema = z.object({
  selectedPollAnswerId: z.number({
    message: "SelectedPollAnswerId is required.",
  }),
});
