import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useCandidateMutations } from "../../hooks/useCandidates";
import { CandidateCard } from "../../components/candidates/CandidateCard";
import type { CandidateResponse } from "../../types/candidate.types";

export const CandidateNominatePage = () => {
  const navigate = useNavigate();
  const [nationalId, setNationalId] = useState("");
  const [nominatedCandidate, setNominatedCandidate] =
    useState<CandidateResponse | null>(null);

  const { addCandidate, isAdding } = useCandidateMutations();

  const handleNominate = (e: React.FormEvent) => {
    e.preventDefault();
    const trimmed = nationalId.trim();
    if (!trimmed) return;

    addCandidate(
      { nationalId: trimmed },
      {
        onSuccess: (data) => {
          setNominatedCandidate(data);
          setNationalId("");
        },
      },
    );
  };

  return (
    <div className="space-y-6">
      <div className="border-b border-slate-200 pb-4">
        <h1 className="text-xl font-bold text-slate-900">
          Nominate Citizen as Candidate
        </h1>
        <p className="text-xs text-slate-500 mt-1">
          Enter an eligible Citizen's National ID to register them and issue
          their nomination credentials.
        </p>
      </div>

      {/* Single Input Form */}
      <form onSubmit={handleNominate} className="flex gap-2 max-w-md">
        <input
          type="text"
          placeholder="Enter Citizen National ID..."
          value={nationalId}
          onChange={(e) => setNationalId(e.target.value)}
          className="flex-1 rounded-lg border border-slate-300 p-2.5 text-xs focus:border-slate-800 focus:outline-none"
        />
        <button
          type="submit"
          disabled={isAdding || !nationalId.trim()}
          className="rounded-lg bg-slate-900 px-4 py-2.5 text-xs font-semibold text-white hover:bg-slate-800 disabled:opacity-50"
        >
          {isAdding ? "Nominating..." : "Nominate Candidate"}
        </button>
      </form>

      {/* Display Card on Success */}
      {nominatedCandidate && (
        <div className="space-y-3 pt-2">
          <p className="text-xs font-semibold text-purple-700">
            ✓ Citizen successfully nominated! Official candidate credentials:
          </p>
          <CandidateCard candidate={nominatedCandidate}>
            <button
              onClick={() => navigate("/candidates")}
              className="rounded bg-slate-900 px-3 py-1.5 text-xs font-semibold text-white hover:bg-slate-800"
            >
              Back to Candidates List
            </button>
            <button
              onClick={() => setNominatedCandidate(null)}
              className="rounded border border-slate-300 px-3 py-1.5 text-xs text-slate-600 hover:bg-slate-50"
            >
              Nominate Another
            </button>
          </CandidateCard>
        </div>
      )}
    </div>
  );
};
