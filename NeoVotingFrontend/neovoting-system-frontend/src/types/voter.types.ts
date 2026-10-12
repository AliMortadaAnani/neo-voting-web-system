// types/voter.type.ts
import { z } from "zod";
import {
  voterCastElectionSchema,
  voterCastPollSchema,
} from "../schemas/voter.schemas";

export type ElectionVoteLogResponseDTO = {
  voteId: string;
  timestampUTC: string;
};

export type PollVoteLogResponseDTO = {
  voteId: string;
  timestampUTC: string;
};

export type VoterCastInElectionRequestDTO = z.infer<
  typeof voterCastElectionSchema
>;
export type VoterCastInPollRequestDTO = z.infer<typeof voterCastPollSchema>;
