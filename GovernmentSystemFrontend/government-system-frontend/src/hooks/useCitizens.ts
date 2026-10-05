import {
  useQuery,
  useMutation,
  useQueryClient,
  keepPreviousData,
} from "@tanstack/react-query";

import toast from "react-hot-toast";

import type { AppError } from "../api/api";

import type {
  CitizenResponseDTO,
  PagedResult,
  CitizenCreateRequestDTO,
  CitizenUpdateRequestDTO,
  CitizenFetchRequestDTO,
} from "../types/citizen.types";
import {
  getCitizensPaged,
  getCitizenDetails,
  getCitizensTotalCount,
  createCitizen,
  updateCitizen,
  deleteCitizen,
} from "../services/citizen.services";

import { voterKeys } from "./useVoters";
import { candidateKeys } from "./useCandidates";

export const citizenKeys = {
  all: ["citizens"] as const,
  lists: () => [...citizenKeys.all, "list"] as const,
  list: (page: number, pageSize: number) =>
    [...citizenKeys.lists(), { page, pageSize }] as const,
  details: () => [...citizenKeys.all, "detail"] as const,
  detail: (nationalId: string) =>
    [...citizenKeys.details(), nationalId] as const,
  totalCount: () => [...citizenKeys.all, "total-count"] as const,
};

export const useCitizens = (page = 1, pageSize = 10) => {
  return useQuery<PagedResult<CitizenResponseDTO>, AppError>({
    queryKey: citizenKeys.list(page, pageSize),
    queryFn: () => getCitizensPaged(page, pageSize),
    placeholderData: keepPreviousData,
    staleTime: 1000 * 30,
  });
};

export const useCitizen = (dto: CitizenFetchRequestDTO | null) => {
  return useQuery<CitizenResponseDTO, AppError>({
    queryKey: citizenKeys.detail(dto?.nationalId ?? ""),
    queryFn: () => getCitizenDetails(dto!),
    enabled: Boolean(dto?.nationalId && dto?.nationalId.trim().length > 0),
    staleTime: 1000 * 60,
  });
};

export const useCitizensTotalCount = () => {
  return useQuery<number, AppError>({
    queryKey: citizenKeys.totalCount(),
    queryFn: getCitizensTotalCount,
    staleTime: 1000 * 60 * 5,
  });
};

export const useCitizenMutations = () => {
  const queryClient = useQueryClient();

  const invalidateLists = () =>
    queryClient.invalidateQueries({ queryKey: citizenKeys.lists() });

  const invalidateTotalCount = () =>
    queryClient.invalidateQueries({ queryKey: citizenKeys.totalCount() });

  const invalidateVoters = () =>
    queryClient.invalidateQueries({ queryKey: voterKeys.all });

  const invalidateCandidates = () =>
    queryClient.invalidateQueries({ queryKey: candidateKeys.all });

  const createMutation = useMutation<
    CitizenResponseDTO,
    AppError,
    CitizenCreateRequestDTO
  >({
    mutationFn: createCitizen,
    onSuccess: () => {
      invalidateLists();
      invalidateTotalCount();
      toast.success("Citizen created successfully");
    },
    onError: (error: AppError) => {
      toast.error(error.message || "Failed to create citizen");
    },
  });

  const updateMutation = useMutation<
    CitizenResponseDTO,
    AppError,
    CitizenUpdateRequestDTO
  >({
    mutationFn: updateCitizen,
    onSuccess: (data, variables) => {
      queryClient.setQueryData(citizenKeys.detail(variables.nationalId), data);
      invalidateLists();
      invalidateVoters();
      invalidateCandidates();
      toast.success("Citizen updated successfully");
    },
    onError: (error: AppError) => {
      toast.error(error.message || "Failed to update citizen");
    },
  });

  const deleteMutation = useMutation<boolean, AppError, CitizenFetchRequestDTO>(
    {
      mutationFn: deleteCitizen,
      onSuccess: (_, variables) => {
        invalidateLists();
        invalidateTotalCount();
        queryClient.removeQueries({
          queryKey: citizenKeys.detail(variables.nationalId),
        });
        invalidateVoters();
        invalidateCandidates();

        toast.success("Citizen deleted successfully");
      },
      onError: (error: AppError) => {
        toast.error(error.message || "Failed to delete citizen");
      },
    },
  );

  return {
    createCitizen: createMutation.mutate,
    createCitizenAsync: createMutation.mutateAsync,
    isCreatingCitizen: createMutation.isPending,
    isCreateCitizenError: createMutation.isError,
    createCitizenError: createMutation.error,

    updateCitizen: updateMutation.mutate,
    updateCitizenAsync: updateMutation.mutateAsync,
    isUpdatingCitizen: updateMutation.isPending,
    isUpdateCitizenError: updateMutation.isError,
    updateCitizenError: updateMutation.error,

    deleteCitizen: deleteMutation.mutate,
    deleteCitizenAsync: deleteMutation.mutateAsync,
    isDeletingCitizen: deleteMutation.isPending,
    isDeleteCitizenError: deleteMutation.isError,
    deleteCitizenError: deleteMutation.error,
  };
};
