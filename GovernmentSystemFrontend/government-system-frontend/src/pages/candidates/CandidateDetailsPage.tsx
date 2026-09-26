import { useState } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { useCandidate } from "../../hooks/useCandidates";
import { CandidateCard } from "../../components/candidates/CandidateCard";

export const CandidateDetailsPage = () => {
  const navigate = useNavigate();
  const location = useLocation();

  const state = location.state as { nationalId?: string } | null;
  const initialId = state?.nationalId ?? "";

  const [lookupId, setLookupId] = useState(initialId);
  const [activeId, setActiveId] = useState(initialId);

  const { data: candidate, isLoading, isError } = useCandidate(activeId);

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    const trimmed = lookupId.trim();
    if (trimmed) {
      setActiveId(trimmed);
      navigate("/candidates/details", {
        state: { nationalId: trimmed },
        replace: true,
      });
    }
  };

  const handleClear = () => {
    setLookupId("");
    setActiveId("");
    navigate("/candidates/details", { state: null, replace: true });
  };

  return (
    <div className="space-y-6">
      <div className="border-b border-slate-200 pb-4">
        <h1 className="text-xl font-bold text-slate-900">
          Candidate Profile & Credentials
        </h1>
        <p className="text-xs text-slate-500 mt-1">
          Inspect candidate registry, nomination token, and security hash.
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
        {activeId && (
          <button
            type="button"
            onClick={handleClear}
            className="rounded-lg border border-slate-300 px-3 py-2 text-xs text-slate-600 hover:bg-slate-50"
          >
            Clear
          </button>
        )}
      </form>

      {isLoading && (
        <p className="text-xs text-slate-500">Loading candidate record...</p>
      )}
      {isError && (
        <p className="text-xs text-red-600">
          No candidate found for National ID [{activeId}].
        </p>
      )}

      {candidate && (
        <CandidateCard candidate={candidate}>
          <button
            onClick={() =>
              navigate("/candidates/token", {
                state: { nationalId: candidate.nationalId },
              })
            }
            className="rounded bg-slate-900 px-3 py-1.5 text-xs font-semibold text-white hover:bg-slate-800"
          >
            Regenerate Token
          </button>
          <button
            onClick={() =>
              navigate("/candidates/delete", {
                state: { nationalId: candidate.nationalId },
              })
            }
            className="rounded border border-red-200 px-3 py-1.5 text-xs font-semibold text-red-600 hover:bg-red-50"
          >
            Delete Candidate
          </button>
          <button
            onClick={() => navigate("/candidates")}
            className="rounded border border-slate-200 px-3 py-1.5 text-xs text-slate-600 hover:bg-slate-50"
          >
            Back to List
          </button>
        </CandidateCard>
      )}
    </div>
  );
};
