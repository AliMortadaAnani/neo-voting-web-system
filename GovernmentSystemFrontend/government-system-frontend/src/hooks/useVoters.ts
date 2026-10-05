import {
  useQuery,
  useMutation,
  useQueryClient,
  keepPreviousData,
} from "@tanstack/react-query";
import toast from "react-hot-toast";
import type { AppError } from "../api/api";
import type {
  VoterResponseDTO,
  PagedResult,
  VoterRequestDTO,
} from "../types/voter.types";
import {
  getVotersPaged,
  getVoterDetails,
  getVotersTotalCount,
  addVoter,
  regenerateVotingToken,
  deleteVoter,
} from "../services/voter.services";

export const voterKeys = {
  all: ["voters"] as const,
  lists: () => [...voterKeys.all, "list"] as const,
  list: (page: number, pageSize: number) =>
    [...voterKeys.lists(), { page, pageSize }] as const,
  details: () => [...voterKeys.all, "detail"] as const,
  detail: (nationalId: string) => [...voterKeys.details(), nationalId] as const,
  totalCount: () => [...voterKeys.all, "total-count"] as const,
};

export const useVoters = (page = 1, pageSize = 10) => {
  return useQuery<PagedResult<VoterResponseDTO>, AppError>({
    queryKey: voterKeys.list(page, pageSize),
    queryFn: () => getVotersPaged(page, pageSize),
    placeholderData: keepPreviousData,
    staleTime: 1000 * 30,
  });
};

export const useVoter = (dto: VoterRequestDTO | null) => {
  return useQuery<VoterResponseDTO, AppError>({
    queryKey: voterKeys.detail(dto?.nationalId ?? ""),
    queryFn: () => getVoterDetails(dto!),
    enabled: Boolean(dto?.nationalId && dto?.nationalId.trim().length > 0),
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

export const useVoterMutations = () => {
  const queryClient = useQueryClient();

  const invalidateLists = () =>
    queryClient.invalidateQueries({ queryKey: voterKeys.lists() });

  const invalidateTotalCount = () =>
    queryClient.invalidateQueries({ queryKey: voterKeys.totalCount() });

  const addMutation = useMutation<VoterResponseDTO, AppError, VoterRequestDTO>({
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

  const tokenMutation = useMutation<
    VoterResponseDTO,
    AppError,
    VoterRequestDTO
  >({
    mutationFn: regenerateVotingToken,
    onSuccess: (data, variables) => {
      invalidateLists();
      queryClient.setQueryData(voterKeys.detail(variables.nationalId), data);
      toast.success("New voting token generated successfully");
    },
    onError: (error: AppError) => {
      toast.error(error.message || "Failed to generate new voting token");
    },
  });

  const deleteMutation = useMutation<boolean, AppError, VoterRequestDTO>({
    mutationFn: deleteVoter,
    onSuccess: (_, variables) => {
      invalidateLists();
      invalidateTotalCount();
      queryClient.removeQueries({
        queryKey: voterKeys.detail(variables.nationalId),
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
    isAddingVoter: addMutation.isPending,
    isAddVoterError: addMutation.isError,
    addVoterError: addMutation.error,

    regenerateVotingToken: tokenMutation.mutate,
    regenerateVotingTokenAsync: tokenMutation.mutateAsync,
    isRegeneratingVotingToken: tokenMutation.isPending,
    isRegenerateVotingTokenError: tokenMutation.isError,
    regenerateVotingTokenError: tokenMutation.error,

    deleteVoter: deleteMutation.mutate,
    deleteVoterAsync: deleteMutation.mutateAsync,
    isDeletingVoter: deleteMutation.isPending,
    isDeleteVoterError: deleteMutation.isError,
    deleteVoterError: deleteMutation.error,
  };
};
