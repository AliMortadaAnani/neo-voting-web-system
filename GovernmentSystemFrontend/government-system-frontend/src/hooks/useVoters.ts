import {
  useQuery,
  useMutation,
  useQueryClient,
  keepPreviousData,
} from "@tanstack/react-query";
import toast from "react-hot-toast";
import type { AppError } from "../api/api";
import type {
  VoterResponse,
  PagedResult,
  CreateVoterDTO,
  UpdateVoterDTO,
} from "../types/voter.types";
import {
  getVotersPaged,
  getVoterDetails,
  getVotersTotalCount,
  addVoter,
  regenerateVotingToken,
  deleteVoter,
} from "../services/voter.services";

// 1. Query Key Factory
export const voterKeys = {
  all: ["voters"] as const,
  lists: () => [...voterKeys.all, "list"] as const,
  list: (page: number, pageSize: number) =>
    [...voterKeys.lists(), { page, pageSize }] as const,
  details: () => [...voterKeys.all, "detail"] as const,
  detail: (nationalId: string) => [...voterKeys.details(), nationalId] as const,
  totalCount: () => [...voterKeys.all, "total-count"] as const,
};

// 2. Read Hooks
export const useVoters = (page = 1, pageSize = 10) => {
  return useQuery<PagedResult<VoterResponse>, AppError>({
    queryKey: voterKeys.list(page, pageSize),
    queryFn: () => getVotersPaged(page, pageSize),
    placeholderData: keepPreviousData,
    staleTime: 1000 * 30,
  });
};

export const useVoter = (nationalId?: string | null) => {
  return useQuery<VoterResponse, AppError>({
    queryKey: voterKeys.detail(nationalId ?? ""),
    queryFn: () => getVoterDetails(nationalId!),
    enabled: Boolean(nationalId && nationalId.trim().length > 0),
    staleTime: 1000 * 60,
  });
};

export const useVotersTotalCount = () => {
  return useQuery<number, AppError>({
    queryKey: voterKeys.totalCount(),
    queryFn: getVotersTotalCount,
    staleTime: 1000 * 60 * 5,
  });
};

// 3. Write Hooks
export const useVoterMutations = () => {
  const queryClient = useQueryClient();

  const invalidateLists = () =>
    queryClient.invalidateQueries({ queryKey: voterKeys.lists() });

  const invalidateTotalCount = () =>
    queryClient.invalidateQueries({ queryKey: voterKeys.totalCount() });

  // Add Voter
  const addMutation = useMutation<VoterResponse, AppError, CreateVoterDTO>({
    mutationFn: addVoter,
    onSuccess: () => {
      invalidateLists();
      invalidateTotalCount();
      toast.success("Citizen enrolled as a voter successfully");
    },
    onError: (error: AppError) => {
      toast.error(error.message || "Failed to enroll voter");
    },
  });

  // Regenerate Token
  const tokenMutation = useMutation<VoterResponse, AppError, UpdateVoterDTO>({
    mutationFn: regenerateVotingToken,
    onSuccess: (data, variables) => {
      invalidateLists();
      // Instantly update the cache with the new token
      queryClient.setQueryData(voterKeys.detail(variables.nationalId), data);
      toast.success("New voting token generated successfully");
    },
    onError: (error: AppError) => {
      toast.error(error.message || "Failed to generate new voting token");
    },
  });

  // Delete Voter
  const deleteMutation = useMutation<boolean, AppError, string>({
    mutationFn: deleteVoter,
    onSuccess: (_, nationalId) => {
      invalidateLists();
      invalidateTotalCount();
      queryClient.removeQueries({
        queryKey: voterKeys.detail(nationalId),
      });
      toast.success("Voter record removed successfully");
    },
    onError: (error: AppError) => {
      toast.error(error.message || "Failed to remove voter");
    },
  });

  return {
    addVoter: addMutation.mutate,
    addVoterAsync: addMutation.mutateAsync,
    isAdding: addMutation.isPending,
    addError: addMutation.error,

    regenerateToken: tokenMutation.mutate,
    regenerateTokenAsync: tokenMutation.mutateAsync,
    isRegeneratingToken: tokenMutation.isPending,
    regenerateTokenError: tokenMutation.error,

    deleteVoter: deleteMutation.mutate,
    deleteVoterAsync: deleteMutation.mutateAsync,
    isDeleting: deleteMutation.isPending,
    deleteError: deleteMutation.error,
  };
};
