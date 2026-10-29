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
  console.log(
    "[useVoters] Fetching voters for page:",
    page,
    "pageSize:",
    pageSize,
  );

  const {
    data: voters,
    isLoading: isFetchingVoters,
    isError: isFetchVotersError,
    error: fetchVotersError,
  } = useQuery<PagedResult<VoterResponseDTO>, AppError>({
    queryKey: voterKeys.list(page, pageSize),
    queryFn: () => getVotersPaged(page, pageSize),
    placeholderData: keepPreviousData,
    staleTime: 1000 * 30,
  });

  return {
    voters,
    isFetchingVoters,
    isFetchVotersError,
    fetchVotersError,
  };
};

export const useVoter = (dto: VoterRequestDTO | null) => {
  console.log(
    "[useVoter] Fetching voter details for nationalId:",
    dto?.nationalId,
  );

  const {
    data: voter,
    isLoading: isFetchingVoter,
    isError: isFetchVoterError,
    error: fetchVoterError,
  } = useQuery<VoterResponseDTO, AppError>({
    queryKey: voterKeys.detail(dto?.nationalId ?? ""),
    queryFn: () => getVoterDetails(dto!),
    enabled: Boolean(dto?.nationalId && dto.nationalId.trim().length > 0),
    staleTime: 1000 * 60,
  });

  return {
    voter,
    isFetchingVoter,
    isFetchVoterError,
    fetchVoterError,
  };
};

export const useVotersTotalCount = () => {
  console.log("[useVotersTotalCount] Fetching total voter count");

  const {
    data: votersTotalCount,
    isLoading: isFetchingVotersTotalCount,
    isError: isFetchVotersTotalCountError,
    error: fetchVotersTotalCountError,
  } = useQuery<number, AppError>({
    queryKey: voterKeys.totalCount(),
    queryFn: getVotersTotalCount,
    staleTime: 1000 * 60 * 5,
  });

  return {
    votersTotalCount,
    isFetchingVotersTotalCount,
    isFetchVotersTotalCountError,
    fetchVotersTotalCountError,
  };
};

export const useVoterMutations = () => {
  const queryClient = useQueryClient();

  const invalidateLists = () =>
    queryClient.invalidateQueries({ queryKey: voterKeys.lists() });

  const invalidateTotalCount = () =>
    queryClient.invalidateQueries({ queryKey: voterKeys.totalCount() });

  const addMutation = useMutation<VoterResponseDTO, AppError, VoterRequestDTO>({
    mutationFn: addVoter,
    onSuccess: (data) => {
      invalidateLists();
      invalidateTotalCount();
      toast.success("Citizen enrolled as a voter successfully");
      console.log(
        "[useVoters] Citizen enrolled as a voter successfully:",
        data,
      );
    },
    onError: (error: AppError) => {
      toast.error(error.message || "Failed to enroll voter");
      console.error("[useVoters] Failed to enroll voter, error:", error);
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
      console.log(
        "[useVoters] New voting token generated successfully for nationalId:",
        variables.nationalId,
        "response:",
        data,
      );
    },
    onError: (error: AppError) => {
      toast.error(error.message || "Failed to generate new voting token");
      console.error(
        "[useVoters] Failed to generate new voting token, error:",
        error,
      );
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
      console.log(
        "[useVoters] Voter record removed successfully for nationalId:",
        variables.nationalId,
      );
    },
    onError: (error: AppError) => {
      toast.error(error.message || "Failed to remove voter");
      console.error("[useVoters] Failed to remove voter, error:", error);
    },
  });

  return {
    addVoter: addMutation.mutate,
    isAddingVoter: addMutation.isPending,
    isAddVoterError: addMutation.isError,
    addVoterError: addMutation.error,
    addVoterResponse: addMutation.data,
    regenerateVotingToken: tokenMutation.mutate,
    isRegeneratingVotingToken: tokenMutation.isPending,
    isRegenerateVotingTokenError: tokenMutation.isError,
    regenerateVotingTokenError: tokenMutation.error,
    regenerateVotingTokenResponse: tokenMutation.data,
    deleteVoter: deleteMutation.mutate,
    isDeletingVoter: deleteMutation.isPending,
    isDeleteVoterError: deleteMutation.isError,
    deleteVoterError: deleteMutation.error,
  };
};
