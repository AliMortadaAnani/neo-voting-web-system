import {
  useQuery,
  useMutation,
  useQueryClient,
  keepPreviousData,
} from "@tanstack/react-query";
import toast from "react-hot-toast";
import type { AppError } from "../api/api";
import type {
  CandidateResponse,
  PagedResult,
  CreateCandidateDTO,
  UpdateCandidateDTO,
} from "../types/candidate.types";
import {
  getCandidatesPaged,
  getCandidateDetails,
  getCandidatesTotalCount,
  addCandidate,
  regenerateNominationToken,
  deleteCandidate,
} from "../services/candidate.services";

// 1. Query Key Factory
export const candidateKeys = {
  all: ["candidates"] as const,
  lists: () => [...candidateKeys.all, "list"] as const,
  list: (page: number, pageSize: number) =>
    [...candidateKeys.lists(), { page, pageSize }] as const,
  details: () => [...candidateKeys.all, "detail"] as const,
  detail: (nationalId: string) =>
    [...candidateKeys.details(), nationalId] as const,
  totalCount: () => [...candidateKeys.all, "total-count"] as const,
};

// 2. Read Hooks
export const useCandidates = (page = 1, pageSize = 10) => {
  return useQuery<PagedResult<CandidateResponse>, AppError>({
    queryKey: candidateKeys.list(page, pageSize),
    queryFn: () => getCandidatesPaged(page, pageSize),
    placeholderData: keepPreviousData,
    staleTime: 1000 * 30,
  });
};

export const useCandidate = (nationalId?: string | null) => {
  return useQuery<CandidateResponse, AppError>({
    queryKey: candidateKeys.detail(nationalId ?? ""),
    queryFn: () => getCandidateDetails(nationalId!),
    enabled: Boolean(nationalId && nationalId.trim().length > 0),
    staleTime: 1000 * 60,
  });
};

export const useCandidatesTotalCount = () => {
  return useQuery<number, AppError>({
    queryKey: candidateKeys.totalCount(),
    queryFn: getCandidatesTotalCount,
    staleTime: 1000 * 60 * 5,
  });
};

// 3. Write Hooks
export const useCandidateMutations = () => {
  const queryClient = useQueryClient();

  const invalidateLists = () =>
    queryClient.invalidateQueries({ queryKey: candidateKeys.lists() });

  const invalidateTotalCount = () =>
    queryClient.invalidateQueries({ queryKey: candidateKeys.totalCount() });

  // Add Candidate
  const addMutation = useMutation<
    CandidateResponse,
    AppError,
    CreateCandidateDTO
  >({
    mutationFn: addCandidate,
    onSuccess: () => {
      invalidateLists();
      invalidateTotalCount();
      toast.success("Citizen nominated as a candidate successfully");
    },
    onError: (error: AppError) => {
      toast.error(error.message || "Failed to nominate candidate");
    },
  });

  // Regenerate Token
  const tokenMutation = useMutation<
    CandidateResponse,
    AppError,
    UpdateCandidateDTO
  >({
    mutationFn: regenerateNominationToken,
    onSuccess: (data, variables) => {
      invalidateLists();
      // Instantly update the cache with the new token
      queryClient.setQueryData(
        candidateKeys.detail(variables.nationalId),
        data,
      );
      toast.success("New nomination token generated successfully");
    },
    onError: (error: AppError) => {
      toast.error(error.message || "Failed to generate new nomination token");
    },
  });

  // Delete Candidate
  const deleteMutation = useMutation<boolean, AppError, string>({
    mutationFn: deleteCandidate,
    onSuccess: (_, nationalId) => {
      invalidateLists();
      invalidateTotalCount();
      queryClient.removeQueries({
        queryKey: candidateKeys.detail(nationalId),
      });
      toast.success("Candidate record removed successfully");
    },
    onError: (error: AppError) => {
      toast.error(error.message || "Failed to remove candidate");
    },
  });

  return {
    addCandidate: addMutation.mutate,
    addCandidateAsync: addMutation.mutateAsync,
    isAdding: addMutation.isPending,
    addError: addMutation.error,

    regenerateToken: tokenMutation.mutate,
    regenerateTokenAsync: tokenMutation.mutateAsync,
    isRegeneratingToken: tokenMutation.isPending,
    regenerateTokenError: tokenMutation.error,

    deleteCandidate: deleteMutation.mutate,
    deleteCandidateAsync: deleteMutation.mutateAsync,
    isDeleting: deleteMutation.isPending,
    deleteError: deleteMutation.error,
  };
};
