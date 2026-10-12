import { api } from "../api/api";
import type { ElectionResponseDTO } from "../types/admin.types";
import type {
  CandidateProfileResponseDTO,
  PollResponseDTO,
} from "../types/general.types";
import type {
  ElectionVoteLogResponseDTO,
  PollVoteLogResponseDTO,
} from "../types/voter.types";

export const getCandidateProfile = async (
  electionId: number,
  candidateId: number,
): Promise<CandidateProfileResponseDTO> => {
  const response = await api.get<CandidateProfileResponseDTO>(
    `/General/elections/${electionId}/profiles/${candidateId}`,
  );
  return response.data;
};

export const getActiveElection = async (): Promise<ElectionResponseDTO> => {
  const response = await api.get<ElectionResponseDTO>(
    "/General/elections/active",
  );
  return response.data;
};

export const getActivePoll = async (): Promise<PollResponseDTO> => {
  const response = await api.get<PollResponseDTO>("/General/polls/active");
  return response.data;
};

export const getPagedElections = async (
  page = 1,
  pageSize = 10,
): Promise<ElectionResponseDTO[]> => {
  const response = await api.get<ElectionResponseDTO[]>(
    "/General/elections/paged",
    {
      params: { page, pageSize },
    },
  );
  return response.data;
};

export const getPagedPolls = async (
  page = 1,
  pageSize = 10,
): Promise<PollResponseDTO[]> => {
  const response = await api.get<PollResponseDTO[]>("/General/polls/paged", {
    params: { page, pageSize },
  });
  return response.data;
};

export const getPagedNominatedCandidates = async (
  electionId: number,
  pageNumber = 1,
  pageSize = 10,
): Promise<CandidateProfileResponseDTO[]> => {
  const response = await api.get<CandidateProfileResponseDTO[]>(
    `/General/elections/${electionId}/candidates/paged`,
    { params: { pageNumber, pageSize } },
  );
  return response.data;
};

export const getPagedElectionVoteLogs = async (
  electionId: number,
  pageNumber = 1,
  pageSize = 10,
): Promise<ElectionVoteLogResponseDTO[]> => {
  const response = await api.get<ElectionVoteLogResponseDTO[]>(
    `/General/elections/${electionId}/voteLogs`,
    { params: { pageNumber, pageSize } },
  );
  return response.data;
};

export const getPagedPollVoteLogs = async (
  pollId: number,
  pageNumber = 1,
  pageSize = 10,
): Promise<PollVoteLogResponseDTO[]> => {
  const response = await api.get<PollVoteLogResponseDTO[]>(
    `/General/polls/${pollId}/voteLogs`,
    { params: { pageNumber, pageSize } },
  );
  return response.data;
};

export const getPollResults = async (
  pollId: number,
): Promise<PollResponseDTO> => {
  const response = await api.get<PollResponseDTO>(
    `/General/polls/${pollId}/results`,
  );
  return response.data;
};

export const getPagedCandidateResultsForElection = async (
  electionId: number,
  pageNumber = 1,
  pageSize = 10,
): Promise<CandidateProfileResponseDTO[]> => {
  const response = await api.get<CandidateProfileResponseDTO[]>(
    `/General/elections/${electionId}/candidateResults`,
    { params: { pageNumber, pageSize } },
  );
  return response.data;
};
