import { api } from "../api/api";
import type {
  VoterCastInElectionRequestDTO,
  VoterCastInPollRequestDTO,
  ElectionVoteLogResponseDTO,
  PollVoteLogResponseDTO,
} from "../types/voter.types";

export const castVoteInElection = async (
  electionId: number,
  dto: VoterCastInElectionRequestDTO,
): Promise<ElectionVoteLogResponseDTO> => {
  const response = await api.post<ElectionVoteLogResponseDTO>(
    `/Voter/elections/${electionId}/vote`,
    dto,
  );
  return response.data;
};

export const castVoteInPoll = async (
  pollId: number,
  dto: VoterCastInPollRequestDTO,
): Promise<PollVoteLogResponseDTO> => {
  const response = await api.post<PollVoteLogResponseDTO>(
    `/Voter/polls/${pollId}/vote`,
    dto,
  );
  return response.data;
};
