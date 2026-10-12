import { useMutation, useQueryClient } from "@tanstack/react-query";
import toast from "react-hot-toast";
import type { AppError } from "../api/api";
import type {
  VoterCastInElectionRequestDTO,
  VoterCastInPollRequestDTO,
  ElectionVoteLogResponseDTO,
  PollVoteLogResponseDTO,
} from "../types/voter.types";
import { castVoteInElection, castVoteInPoll } from "../services/voter.services";

export const useVoterMutations = () => {
  const queryClient = useQueryClient();

  const castElectionVoteMutation = useMutation<
    ElectionVoteLogResponseDTO,
    AppError,
    { electionId: number; dto: VoterCastInElectionRequestDTO }
  >({
    mutationFn: ({ electionId, dto }) => castVoteInElection(electionId, dto),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({
        queryKey: ["voteLogs", "election", variables.electionId],
      });
      toast.success("Vote cast successfully in election!");
    },
    onError: (error) => {
      toast.error(error.message || "Failed to cast vote in election");
    },
  });

  const castPollVoteMutation = useMutation<
    PollVoteLogResponseDTO,
    AppError,
    { pollId: number; dto: VoterCastInPollRequestDTO }
  >({
    mutationFn: ({ pollId, dto }) => castVoteInPoll(pollId, dto),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({
        queryKey: ["voteLogs", "poll", variables.pollId],
      });
      queryClient.invalidateQueries({
        queryKey: ["polls", variables.pollId, "results"],
      });
      toast.success("Vote cast successfully in poll!");
    },
    onError: (error) => {
      toast.error(error.message || "Failed to cast vote in poll");
    },
  });

  return {
    castElectionVote: castElectionVoteMutation.mutate,
    castElectionVoteAsync: castElectionVoteMutation.mutateAsync,
    isVotingElection: castElectionVoteMutation.isPending,

    castPollVote: castPollVoteMutation.mutate,
    castPollVoteAsync: castPollVoteMutation.mutateAsync,
    isVotingPoll: castPollVoteMutation.isPending,
  };
};
