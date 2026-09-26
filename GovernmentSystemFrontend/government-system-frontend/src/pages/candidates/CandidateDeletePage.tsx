import { useState } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { useCandidate, useCandidateMutations } from "../../hooks/useCandidates";
import { CandidateCard } from "../../components/candidates/CandidateCard";

export const CandidateDeletePage = () => {
  const navigate = useNavigate();
  const location = useLocation();

  const state = location.state as { nationalId?: string } | null;
  const initialId = state?.nationalId ?? "";

  const [lookupId, setLookupId] = useState(initialId);
  const [activeId, setActiveId] = useState(initialId);

  const { data: candidate, isLoading, isError } = useCandidate(activeId);
  const { deleteCandidate, isDeleting } = useCandidateMutations();

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    const trimmed = lookupId.trim();
    if (trimmed) {
      setActiveId(trimmed);
      navigate("/candidates/delete", {
        state: { nationalId: trimmed },
        replace: true,
      });
    }
  };

  const handleDelete = () => {
    if (!activeId) return;

    deleteCandidate(activeId, {
      onSuccess: () => {
        navigate("/candidates");
      },
    });
  };

  return (
    <div className="space-y-6">
      <div className="border-b border-red-200 pb-4">
        <h1 className="text-xl font-bold text-red-700">
          Remove Candidate Nomination
        </h1>
        <p className="text-xs text-slate-500 mt-1">
          Removes this citizen's nomination credentials. Their Citizen record
          will remain intact.
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
              onClick={handleDelete}
              disabled={isDeleting}
              className="rounded bg-red-600 px-3 py-1.5 text-xs font-semibold text-white hover:bg-red-700 disabled:opacity-50"
            >
              {isDeleting ? "Deleting..." : "Confirm & Remove Candidate"}
            </button>
            <button
              onClick={() => navigate("/candidates")}
              className="rounded border border-slate-300 px-3 py-1.5 text-xs text-slate-700 hover:bg-slate-50"
            >
              Cancel
            </button>
          </CandidateCard>
        </div>
      )}
    </div>
  );
};
