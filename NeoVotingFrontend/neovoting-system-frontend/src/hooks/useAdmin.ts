import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import toast from "react-hot-toast";
import type { AppError } from "../api/api";
import type {
  ElectionCreateRequestDTO,
  ElectionResponseDTO,
  PollCreateRequestDTO,
  SystemAuditLogResponseDTO,
} from "../types/admin.types";
import type { PollResponseDTO } from "../types/general.types";
import {
  createElection,
  createPoll,
  startElection,
  completeElection,
  startPoll,
  completePoll,
  getPagedSystemAuditLogs,
} from "../services/admin.services";

export const adminKeys = {
  all: ["admin"] as const,
  auditLogs: (
    actionType?: number,
    adminId?: number,
    page?: number,
    size?: number,
  ) =>
    [
      ...adminKeys.all,
      "audit-logs",
      { actionType, adminId, page, size },
    ] as const,
};

export const useSystemAuditLogs = (
  actionType?: number,
  adminId?: number,
  pageNumber = 1,
  pageSize = 10,
) => {
  return useQuery<SystemAuditLogResponseDTO[], AppError>({
    queryKey: adminKeys.auditLogs(actionType, adminId, pageNumber, pageSize),
    queryFn: () =>
      getPagedSystemAuditLogs(actionType, adminId, pageNumber, pageSize),
    staleTime: 1000 * 30,
  });
};

export const useAdminMutations = () => {
  const queryClient = useQueryClient();

  const createElectionMutation = useMutation<
    ElectionResponseDTO,
    AppError,
    ElectionCreateRequestDTO
  >({
    mutationFn: createElection,
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: ["elections"] });
      toast.success(`Election "${data.name}" created successfully!`);
    },
    onError: (error) =>
      toast.error(error.message || "Failed to create election"),
  });

  const createPollMutation = useMutation<
    PollResponseDTO,
    AppError,
    PollCreateRequestDTO
  >({
    mutationFn: createPoll,
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: ["polls"] });
      toast.success(`Poll "${data.name}" created successfully!`);
    },
    onError: (error) => toast.error(error.message || "Failed to create poll"),
  });

  const startElectionMutation = useMutation<
    ElectionResponseDTO,
    AppError,
    number
  >({
    mutationFn: startElection,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["elections"] });
      toast.success("Election started successfully.");
    },
    onError: (error) =>
      toast.error(error.message || "Failed to start election"),
  });

  const completeElectionMutation = useMutation<
    ElectionResponseDTO,
    AppError,
    number
  >({
    mutationFn: completeElection,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["elections"] });
      toast.success("Election completed successfully.");
    },
    onError: (error) =>
      toast.error(error.message || "Failed to complete election"),
  });

  const startPollMutation = useMutation<PollResponseDTO, AppError, number>({
    mutationFn: startPoll,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["polls"] });
      toast.success("Poll started successfully.");
    },
    onError: (error) => toast.error(error.message || "Failed to start poll"),
  });

  const completePollMutation = useMutation<PollResponseDTO, AppError, number>({
    mutationFn: completePoll,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["polls"] });
      toast.success("Poll completed successfully.");
    },
    onError: (error) => toast.error(error.message || "Failed to complete poll"),
  });

  return {
    createElection: createElectionMutation.mutate,
    createElectionAsync: createElectionMutation.mutateAsync,
    isCreatingElection: createElectionMutation.isPending,

    createPoll: createPollMutation.mutate,
    createPollAsync: createPollMutation.mutateAsync,
    isCreatingPoll: createPollMutation.isPending,

    startElection: startElectionMutation.mutate,
    isStartingElection: startElectionMutation.isPending,

    completeElection: completeElectionMutation.mutate,
    isCompletingElection: completeElectionMutation.isPending,

    startPoll: startPollMutation.mutate,
    isStartingPoll: startPollMutation.isPending,

    completePoll: completePollMutation.mutate,
    isCompletingPoll: completePollMutation.isPending,
  };
};
