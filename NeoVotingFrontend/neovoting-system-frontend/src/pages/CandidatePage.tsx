import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { candidateProfileCreateSchema } from "../schemas/candidates.schemas";
import type { CandidateProfileCreateRequestDTO } from "../types/candidate.types";
import { useCandidateProfileMutations } from "../hooks/useCandidateProfile";
import {
  usePagedElections,
  useCandidateResultsForElection,
} from "../hooks/useGeneral";

export const CandidatePage = () => {
  const [selectedElectionId, setSelectedElectionId] = useState<number>(1);
  const { data: elections } = usePagedElections(1, 10);
  const { data: results } = useCandidateResultsForElection(selectedElectionId);

  const { createProfile, isCreatingProfile } = useCandidateProfileMutations();

  const form = useForm<CandidateProfileCreateRequestDTO>({
    resolver: zodResolver(candidateProfileCreateSchema),
    defaultValues: { nominationReasons: "" },
  });

  const onSubmit = (data: CandidateProfileCreateRequestDTO) => {
    createProfile({ electionId: selectedElectionId, dto: data });
  };

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-2xl font-bold tracking-tight text-slate-900">
          Candidate Portal
        </h1>
        <p className="text-sm text-slate-500">
          Submit your candidacy profile and check election statistics.
        </p>
      </div>

      <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
        {/* Profile Submission Form */}
        <div className="rounded-xl border border-slate-200 bg-white p-6 shadow-xs">
          <h3 className="mb-4 text-lg font-semibold">
            Submit Candidacy Profile
          </h3>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
            <div>
              <label className="mb-1 block text-xs font-medium">
                Select Election
              </label>
              <select
                value={selectedElectionId}
                onChange={(e) => setSelectedElectionId(Number(e.target.value))}
                className="w-full rounded border px-3 py-2 text-sm"
              >
                {elections?.map((el) => (
                  <option key={el.id} value={el.id}>
                    {el.name}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <label className="mb-1 block text-xs font-medium">
                Nomination Reasons
              </label>
              <textarea
                rows={4}
                {...form.register("nominationReasons")}
                className="w-full rounded border px-3 py-2 text-sm"
              />
            </div>
            <button
              type="submit"
              disabled={isCreatingProfile}
              className="w-full rounded bg-slate-900 py-2 text-sm text-white"
            >
              {isCreatingProfile ? "Submitting..." : "Submit Profile"}
            </button>
          </form>
        </div>

        {/* Election Candidate Results Summary */}
        <div className="rounded-xl border border-slate-200 bg-white p-6 shadow-xs">
          <h3 className="mb-4 text-lg font-semibold">Candidate Standings</h3>
          <div className="space-y-2 text-xs">
            {results?.map((res) => (
              <div
                key={res.candidateProfileId}
                className="flex justify-between rounded bg-slate-50 p-2"
              >
                <span>
                  {res.firstName} {res.lastName}
                </span>
                <span className="font-bold">{res.votesCount ?? 0} Votes</span>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
};

export default CandidatePage;
