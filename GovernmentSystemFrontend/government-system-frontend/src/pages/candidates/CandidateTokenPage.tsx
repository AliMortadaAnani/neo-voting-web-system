import { useState } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { useCandidate, useCandidateMutations } from "../../hooks/useCandidates";
import { CandidateCard } from "../../components/candidates/CandidateCard";

export const CandidateTokenPage = () => {
  const navigate = useNavigate();
  const location = useLocation();

  const state = location.state as { nationalId?: string } | null;
  const initialId = state?.nationalId ?? "";

  const [lookupId, setLookupId] = useState(initialId);
  const [activeId, setActiveId] = useState(initialId);

  const { data: candidate, isLoading, isError } = useCandidate(activeId);
  const { regenerateToken, isRegeneratingToken } = useCandidateMutations();

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    const trimmed = lookupId.trim();
    if (trimmed) {
      setActiveId(trimmed);
      navigate("/candidates/token", {
        state: { nationalId: trimmed },
        replace: true,
      });
    }
  };

  const handleGenerate = () => {
    if (!activeId) return;
    regenerateToken({ nationalId: activeId });
  };

  return (
    <div className="space-y-6">
      <div className="border-b border-slate-200 pb-4">
        <h1 className="text-xl font-bold text-slate-900">
          Regenerate Nomination Token
        </h1>
        <p className="text-xs text-slate-500 mt-1">
          Issue a new secure nomination token. The previous token will become
          invalid.
        </p>
      </div>

      {/* Search Bar */}
      <form onSubmit={handleSearch} className="flex gap-2 max-w-md">
        <input
          type="text"
          placeholder="Enter National ID..."
          value={lookupId}
          onChange={(e) => setLookupId(e.target.value)}
          className="flex-1 rounded-lg border border-slate-300 p-2 text-xs focus:border-slate-800 focus:outline-none"
        />
        <button
          type="submit"
          className="rounded-lg bg-slate-900 px-3 py-2 text-xs font-semibold text-white hover:bg-slate-800"
        >
          Search
        </button>
      </form>

      {isLoading && (
        <p className="text-xs text-slate-500">Locating candidate...</p>
      )}
      {isError && (
        <p className="text-xs text-red-600">
          No candidate found for National ID [{activeId}].
        </p>
      )}

      {candidate && (
        <div className="space-y-4">
          <CandidateCard candidate={candidate}>
            <button
              onClick={handleGenerate}
              disabled={isRegeneratingToken}
              className="rounded bg-purple-600 px-3 py-1.5 text-xs font-semibold text-white hover:bg-purple-700 disabled:opacity-50"
            >
              {isRegeneratingToken ? "Generating..." : "⚡ Generate New Token"}
            </button>
            <button
              onClick={() => navigate("/candidates")}
              className="rounded border border-slate-200 px-3 py-1.5 text-xs text-slate-600 hover:bg-slate-50"
            >
              Back to List
            </button>
          </CandidateCard>
        </div>
      )}
    </div>
  );
};
