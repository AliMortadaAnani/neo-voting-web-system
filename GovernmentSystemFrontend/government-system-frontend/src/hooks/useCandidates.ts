import {
  useQuery,
  useMutation,
  useQueryClient,
  keepPreviousData,
} from "@tanstack/react-query";
import toast from "react-hot-toast";
import type { AppError } from "../api/api";
import type {
  CandidateResponseDTO,
  PagedResult,
  CandidateRequestDTO,
} from "../types/candidate.types";
import {
  getCandidatesPaged,
  getCandidateDetails,
  getCandidatesTotalCount,
  addCandidate,
  regenerateNominationToken,
  deleteCandidate,
} from "../services/candidate.services";

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

export const useCandidates = (page = 1, pageSize = 10) => {
  console.log(
    "[useCandidates] Fetching candidates for page:",
    page,
    "pageSize:",
    pageSize,
  );
  return useQuery<PagedResult<CandidateResponseDTO>, AppError>({
    queryKey: candidateKeys.list(page, pageSize),
    queryFn: () => getCandidatesPaged(page, pageSize),
    placeholderData: keepPreviousData,
    staleTime: 1000 * 30,
  });
};

export const useCandidate = (dto: CandidateRequestDTO | null) => {
  console.log(
    "[useCandidate] Fetching candidate details for nationalId:",
    dto?.nationalId,
  );
  return useQuery<CandidateResponseDTO, AppError>({
    queryKey: candidateKeys.detail(dto?.nationalId ?? ""),
    queryFn: () => getCandidateDetails(dto!),
    enabled: Boolean(dto?.nationalId && dto?.nationalId.trim().length > 0),
    staleTime: 1000 * 60,
  });
};

export const useCandidatesTotalCount = () => {
  console.log("[useCandidatesTotalCount] Fetching total candidate count");
  return useQuery<number, AppError>({
    queryKey: candidateKeys.totalCount(),
    queryFn: getCandidatesTotalCount,
    staleTime: 1000 * 60 * 5,
  });
};

export const useCandidateMutations = () => {
  const queryClient = useQueryClient();

  const invalidateLists = () =>
    queryClient.invalidateQueries({ queryKey: candidateKeys.lists() });

  const invalidateTotalCount = () =>
    queryClient.invalidateQueries({ queryKey: candidateKeys.totalCount() });

  const addMutation = useMutation<
    CandidateResponseDTO,
    AppError,
    CandidateRequestDTO
  >({
    mutationFn: addCandidate,
    onSuccess: (data) => {
      invalidateLists();
      invalidateTotalCount();
      toast.success("Citizen nominated as a candidate successfully");
      console.log("[useCandidates] Candidate added successfully:", data);
    },
    onError: (error: AppError) => {
      toast.error(error.message || "Failed to nominate candidate");
      console.error("[useCandidates] Failed to add candidate, error:", error);
    },
  });

  const tokenMutation = useMutation<
    CandidateResponseDTO,
    AppError,
    CandidateRequestDTO
  >({
    mutationFn: regenerateNominationToken,
    onSuccess: (data, variables) => {
      invalidateLists();
      queryClient.setQueryData(
        candidateKeys.detail(variables.nationalId),
        data,
      );
      toast.success("New nomination token generated successfully");
      console.log(
        "[useCandidates] Nomination token regenerated successfully:",
        data,
      );
    },
    onError: (error: AppError) => {
      toast.error(error.message || "Failed to generate new nomination token");
      console.error(
        "[useCandidates] Failed to regenerate nomination token, error:",
        error,
      );
    },
  });

  const deleteMutation = useMutation<boolean, AppError, CandidateRequestDTO>({
    mutationFn: deleteCandidate,
    onSuccess: (_, variables) => {
      invalidateLists();
      invalidateTotalCount();
      queryClient.removeQueries({
        queryKey: candidateKeys.detail(variables.nationalId),
      });
      toast.success("Candidate record removed successfully");
      console.log(
        "[useCandidates] Candidate record removed successfully for nationalId:",
        variables.nationalId,
      );
    },
    onError: (error: AppError) => {
      toast.error(error.message || "Failed to remove candidate");
      console.error(
        "[useCandidates] Failed to remove candidate, error:",
        error,
      );
    },
  });

  return {
    addCandidate: addMutation.mutate,
    addCandidateAsync: addMutation.mutateAsync,
    isAdding: addMutation.isPending,
    isAddError: addMutation.isError,
    addError: addMutation.error,
    addCandidateResponse: addMutation.data,
    regenerateToken: tokenMutation.mutate,
    regenerateTokenAsync: tokenMutation.mutateAsync,
    isRegeneratingToken: tokenMutation.isPending,
    isRegenerateTokenError: tokenMutation.isError,
    regenerateTokenError: tokenMutation.error,
    regenerateTokenResponse: tokenMutation.data,
    deleteCandidate: deleteMutation.mutate,
    deleteCandidateAsync: deleteMutation.mutateAsync,
    isDeleting: deleteMutation.isPending,
    isDeleteError: deleteMutation.isError,
    deleteError: deleteMutation.error,
  };
};
