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
  console.log("[useCitizens] Fetching citizens for page:", page, pageSize);

  const {
    data: citizens,
    isLoading: isFetchingCitizens,
    isError: isFetchCitizensError,
    error: fetchCitizensError,
  } = useQuery<PagedResult<CitizenResponseDTO>, AppError>({
    queryKey: citizenKeys.list(page, pageSize),
    queryFn: () => getCitizensPaged(page, pageSize),
    placeholderData: keepPreviousData,
    staleTime: 1000 * 30,
  });

  return {
    citizens,
    isFetchingCitizens,
    isFetchCitizensError,
    fetchCitizensError,
  };
};

export const useCitizen = (dto: CitizenFetchRequestDTO | null) => {
  console.log(
    "[useCitizen] Fetching citizen details for nationalId:",
    dto?.nationalId,
  );

  const {
    data: citizen,
    isLoading: isFetchingCitizen,
    isError: isFetchCitizenError,
    error: fetchCitizenError,
  } = useQuery<CitizenResponseDTO, AppError>({
    queryKey: citizenKeys.detail(dto?.nationalId ?? ""),
    queryFn: () => getCitizenDetails(dto!),
    enabled: Boolean(dto?.nationalId && dto.nationalId.trim().length > 0),
    staleTime: 1000 * 60,
  });

  return {
    citizen,
    isFetchingCitizen,
    isFetchCitizenError,
    fetchCitizenError,
  };
};

export const useCitizensTotalCount = () => {
  console.log("[useCitizensTotalCount] Fetching total citizen count");

  const {
    data: citizensTotalCount,
    isLoading: isFetchingCitizensTotalCount,
    isError: isFetchCitizensTotalCountError,
    error: fetchCitizensTotalCountError,
  } = useQuery<number, AppError>({
    queryKey: citizenKeys.totalCount(),
    queryFn: getCitizensTotalCount,
    staleTime: 1000 * 60 * 5,
  });

  return {
    citizensTotalCount,
    isFetchingCitizensTotalCount,
    isFetchCitizensTotalCountError,
    fetchCitizensTotalCountError,
  };
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
    onSuccess: (data) => {
      invalidateLists();
      invalidateTotalCount();
      toast.success("Citizen created successfully");
      console.log("[useCitizens] Citizen created successfully:", data);
    },
    onError: (error: AppError) => {
      toast.error(error.message || "Failed to create citizen");
      console.error("[useCitizens] Failed to create citizen, error:", error);
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
      console.log("[useCitizens] Citizen updated successfully:", data);
    },
    onError: (error: AppError) => {
      toast.error(error.message || "Failed to update citizen");
      console.error("[useCitizens] Failed to update citizen, error:", error);
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
        console.log(
          "[useCitizens] Citizen deleted successfully for nationalId:",
          variables.nationalId,
        );
      },
      onError: (error: AppError) => {
        toast.error(error.message || "Failed to delete citizen");
        console.error("[useCitizens] Failed to delete citizen, error:", error);
      },
    },
  );

  return {
    createCitizen: createMutation.mutate,
    isCreatingCitizen: createMutation.isPending,
    isCreateCitizenError: createMutation.isError,
    createCitizenError: createMutation.error,
    createCitizenResponse: createMutation.data,
    updateCitizen: updateMutation.mutate,
    isUpdatingCitizen: updateMutation.isPending,
    isUpdateCitizenError: updateMutation.isError,
    updateCitizenError: updateMutation.error,
    updateCitizenResponse: updateMutation.data,
    deleteCitizen: deleteMutation.mutate,
    isDeletingCitizen: deleteMutation.isPending,
    isDeleteCitizenError: deleteMutation.isError,
    deleteCitizenError: deleteMutation.error,
  };
};
