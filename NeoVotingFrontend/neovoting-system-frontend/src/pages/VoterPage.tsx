import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  voterCastElectionSchema,
  voterCastPollSchema,
} from "../schemas/voter.schemas";
import type {
  VoterCastInElectionRequestDTO,
  VoterCastInPollRequestDTO,
} from "../types/voter.types";
import { useVoterMutations } from "../hooks/useVoter";
import {
  usePagedElections,
  usePagedPolls,
  usePagedNominatedCandidates,
  usePollResults,
} from "../hooks/useGeneral";

export const VoterPage = () => {
  const [electionId, setElectionId] = useState<number>(1);
  const [pollId, setPollId] = useState<number>(1);

  const { data: elections } = usePagedElections(1, 10);
  const { data: polls } = usePagedPolls(1, 10);
  const { data: candidates } = usePagedNominatedCandidates(electionId);
  const { data: pollDetails } = usePollResults(pollId);

  const { castElectionVote, castPollVote, isVotingElection, isVotingPoll } =
    useVoterMutations();

  const electionForm = useForm<VoterCastInElectionRequestDTO>({
    resolver: zodResolver(voterCastElectionSchema),
  });

  const pollForm = useForm<VoterCastInPollRequestDTO>({
    resolver: zodResolver(voterCastPollSchema),
  });

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-2xl font-bold tracking-tight text-slate-900">
          Voter Dashboard
        </h1>
        <p className="text-sm text-slate-500">
          Cast your official ballots in active elections and polls.
        </p>
      </div>

      <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
        {/* Cast Election Vote */}
        <div className="rounded-xl border border-slate-200 bg-white p-6 shadow-xs">
          <h3 className="mb-4 text-lg font-semibold">Vote in Election</h3>
          <div className="mb-4">
            <label className="mb-1 block text-xs font-medium">
              Select Election
            </label>
            <select
              value={electionId}
              onChange={(e) => setElectionId(Number(e.target.value))}
              className="w-full rounded border px-3 py-2 text-sm"
            >
              {elections?.map((el) => (
                <option key={el.id} value={el.id}>
                  {el.name}
                </option>
              ))}
            </select>
          </div>

          <form
            onSubmit={electionForm.handleSubmit((d) =>
              castElectionVote({ electionId, dto: d }),
            )}
            className="space-y-4"
          >
            <div>
              <label className="mb-1 block text-xs font-medium">
                Select Candidate
              </label>
              <select
                {...electionForm.register("selectedCandidateProfileId", {
                  valueAsNumber: true,
                })}
                className="w-full rounded border px-3 py-2 text-sm"
              >
                <option value="">Choose candidate...</option>
                {candidates?.map((c) => (
                  <option
                    key={c.candidateProfileId}
                    value={c.candidateProfileId}
                  >
                    {c.firstName} {c.lastName}
                  </option>
                ))}
              </select>
            </div>
            <button
              type="submit"
              disabled={isVotingElection}
              className="w-full rounded bg-slate-900 py-2 text-sm text-white"
            >
              {isVotingElection ? "Submitting Vote..." : "Cast Election Vote"}
            </button>
          </form>
        </div>

        {/* Cast Poll Vote */}
        <div className="rounded-xl border border-slate-200 bg-white p-6 shadow-xs">
          <h3 className="mb-4 text-lg font-semibold">Vote in Poll</h3>
          <div className="mb-4">
            <label className="mb-1 block text-xs font-medium">
              Select Poll
            </label>
            <select
              value={pollId}
              onChange={(e) => setPollId(Number(e.target.value))}
              className="w-full rounded border px-3 py-2 text-sm"
            >
              {polls?.map((p) => (
                <option key={p.id} value={p.id}>
                  {p.name}
                </option>
              ))}
            </select>
            {pollDetails && (
              <p className="mt-2 text-xs font-medium text-slate-700">
                Question: {pollDetails.question}
              </p>
            )}
          </div>

          <form
            onSubmit={pollForm.handleSubmit((d) =>
              castPollVote({ pollId, dto: d }),
            )}
            className="space-y-4"
          >
            <div>
              <label className="mb-1 block text-xs font-medium">
                Select Answer
              </label>
              <select
                {...pollForm.register("selectedPollAnswerId", {
                  valueAsNumber: true,
                })}
                className="w-full rounded border px-3 py-2 text-sm"
              >
                <option value="">Choose answer...</option>
                {pollDetails?.answers?.map((a) => (
                  <option key={a.id} value={a.id!}>
                    {a.answer}
                  </option>
                ))}
              </select>
            </div>
            <button
              type="submit"
              disabled={isVotingPoll}
              className="w-full rounded bg-slate-900 py-2 text-sm text-white"
            >
              {isVotingPoll ? "Submitting Vote..." : "Cast Poll Vote"}
            </button>
          </form>
        </div>
      </div>
    </div>
  );
};

export default VoterPage;
