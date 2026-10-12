import { useMutation, useQueryClient } from "@tanstack/react-query";
import toast from "react-hot-toast";
import type { AppError } from "../api/api";
import type { CandidateProfileCreateRequestDTO } from "../types/candidate.types";
import type { CandidateProfileResponseDTO } from "../types/general.types";
import { createCandidateProfile } from "../services/candidate.services";

export const useCandidateProfileMutations = () => {
  const queryClient = useQueryClient();

  const createProfileMutation = useMutation<
    CandidateProfileResponseDTO,
    AppError,
    { electionId: number; dto: CandidateProfileCreateRequestDTO }
  >({
    mutationFn: ({ electionId, dto }) =>
      createCandidateProfile(electionId, dto),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({
        queryKey: ["candidates", variables.electionId],
      });
      toast.success("Candidate profile submitted successfully!");
    },
    onError: (error) => {
      toast.error(error.message || "Failed to create candidate profile");
    },
  });

  return {
    createProfile: createProfileMutation.mutate,
    createProfileAsync: createProfileMutation.mutateAsync,
    isCreatingProfile: createProfileMutation.isPending,
  };
};
