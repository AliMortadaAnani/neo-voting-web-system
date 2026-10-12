import { useQuery, keepPreviousData } from "@tanstack/react-query";
import type { AppError } from "../api/api";
import type { ElectionResponseDTO } from "../types/admin.types";
import type {
  CandidateProfileResponseDTO,
  PollResponseDTO,
} from "../types/general.types";
import type {
  ElectionVoteLogResponseDTO,
  PollVoteLogResponseDTO,
} from "../types/voter.types";
import {
  getCandidateProfile,
  getActiveElection,
  getActivePoll,
  getPagedElections,
  getPagedPolls,
  getPagedNominatedCandidates,
  getPagedElectionVoteLogs,
  getPagedPollVoteLogs,
  getPollResults,
  getPagedCandidateResultsForElection,
} from "../services/general.services";

export const generalKeys = {
  all: ["general"] as const,
  activeElection: () => [...generalKeys.all, "election", "active"] as const,
  activePoll: () => [...generalKeys.all, "poll", "active"] as const,
  electionsList: (page: number, size: number) =>
    [...generalKeys.all, "elections", { page, size }] as const,
  pollsList: (page: number, size: number) =>
    [...generalKeys.all, "polls", { page, size }] as const,
  candidateProfile: (electionId: number, candidateId: number) =>
    [...generalKeys.all, "candidate-profile", electionId, candidateId] as const,
  candidatesPaged: (electionId: number, page: number, size: number) =>
    [
      ...generalKeys.all,
      "election-candidates",
      electionId,
      { page, size },
    ] as const,
  electionVoteLogs: (electionId: number, page: number, size: number) =>
    [
      ...generalKeys.all,
      "election-vote-logs",
      electionId,
      { page, size },
    ] as const,
  pollVoteLogs: (pollId: number, page: number, size: number) =>
    [...generalKeys.all, "poll-vote-logs", pollId, { page, size }] as const,
  pollResults: (pollId: number) =>
    [...generalKeys.all, "poll-results", pollId] as const,
  candidateResults: (electionId: number, page: number, size: number) =>
    [
      ...generalKeys.all,
      "candidate-results",
      electionId,
      { page, size },
    ] as const,
};

export const useActiveElection = () => {
  return useQuery<ElectionResponseDTO, AppError>({
    queryKey: generalKeys.activeElection(),
    queryFn: getActiveElection,
    staleTime: 1000 * 60,
  });
};

export const useActivePoll = () => {
  return useQuery<PollResponseDTO, AppError>({
    queryKey: generalKeys.activePoll(),
    queryFn: getActivePoll,
    staleTime: 1000 * 60,
  });
};

export const usePagedElections = (page = 1, pageSize = 10) => {
  return useQuery<ElectionResponseDTO[], AppError>({
    queryKey: generalKeys.electionsList(page, pageSize),
    queryFn: () => getPagedElections(page, pageSize),
    placeholderData: keepPreviousData,
    staleTime: 1000 * 30,
  });
};

export const usePagedPolls = (page = 1, pageSize = 10) => {
  return useQuery<PollResponseDTO[], AppError>({
    queryKey: generalKeys.pollsList(page, pageSize),
    queryFn: () => getPagedPolls(page, pageSize),
    placeholderData: keepPreviousData,
    staleTime: 1000 * 30,
  });
};

export const useCandidateProfileDetails = (
  electionId: number,
  candidateId: number,
  enabled = true,
) => {
  return useQuery<CandidateProfileResponseDTO, AppError>({
    queryKey: generalKeys.candidateProfile(electionId, candidateId),
    queryFn: () => getCandidateProfile(electionId, candidateId),
    enabled: enabled && !!electionId && !!candidateId,
    staleTime: 1000 * 60,
  });
};

export const usePagedNominatedCandidates = (
  electionId: number,
  pageNumber = 1,
  pageSize = 10,
  enabled = true,
) => {
  return useQuery<CandidateProfileResponseDTO[], AppError>({
    queryKey: generalKeys.candidatesPaged(electionId, pageNumber, pageSize),
    queryFn: () =>
      getPagedNominatedCandidates(electionId, pageNumber, pageSize),
    enabled: enabled && !!electionId,
    placeholderData: keepPreviousData,
  });
};

export const usePagedElectionVoteLogs = (
  electionId: number,
  pageNumber = 1,
  pageSize = 10,
) => {
  return useQuery<ElectionVoteLogResponseDTO[], AppError>({
    queryKey: generalKeys.electionVoteLogs(electionId, pageNumber, pageSize),
    queryFn: () => getPagedElectionVoteLogs(electionId, pageNumber, pageSize),
    placeholderData: keepPreviousData,
  });
};

export const usePagedPollVoteLogs = (
  pollId: number,
  pageNumber = 1,
  pageSize = 10,
) => {
  return useQuery<PollVoteLogResponseDTO[], AppError>({
    queryKey: generalKeys.pollVoteLogs(pollId, pageNumber, pageSize),
    queryFn: () => getPagedPollVoteLogs(pollId, pageNumber, pageSize),
    placeholderData: keepPreviousData,
  });
};

export const usePollResults = (pollId: number, enabled = true) => {
  return useQuery<PollResponseDTO, AppError>({
    queryKey: generalKeys.pollResults(pollId),
    queryFn: () => getPollResults(pollId),
    enabled: enabled && !!pollId,
  });
};

export const useCandidateResultsForElection = (
  electionId: number,
  pageNumber = 1,
  pageSize = 10,
  enabled = true,
) => {
  return useQuery<CandidateProfileResponseDTO[], AppError>({
    queryKey: generalKeys.candidateResults(electionId, pageNumber, pageSize),
    queryFn: () =>
      getPagedCandidateResultsForElection(electionId, pageNumber, pageSize),
    enabled: enabled && !!electionId,
    placeholderData: keepPreviousData,
  });
};
