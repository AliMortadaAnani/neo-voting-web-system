import { useState } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { useCitizen, useCitizenMutations } from "../../hooks/useCitizens";

export const CitizenDeletePage = () => {
  const navigate = useNavigate();
  const location = useLocation();

  // 1. Read National ID safely from hidden navigation state
  const state = location.state as { nationalId?: string } | null;
  const initialId = state?.nationalId ?? "";

  // 2. Local input state vs. Active search query state
  const [lookupId, setLookupId] = useState<string>(initialId);
  const [activeId, setActiveId] = useState<string>(initialId);

  // 3. React Query: automatically runs if activeId is not empty
  const { data: citizen, isLoading, isError } = useCitizen(activeId);
  const { deleteCitizen, isDeleting } = useCitizenMutations();

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    const trimmedId = lookupId.trim();

    if (trimmedId) {
      setActiveId(trimmedId);

      // Keep location.state synced so browser refresh (F5) keeps the card open
      navigate("/citizens/delete", {
        state: { nationalId: trimmedId },
        replace: true,
      });
    }
  };

  const handleClear = () => {
    setLookupId("");
    setActiveId("");
    navigate("/citizens/delete", { state: null, replace: true });
  };

  const handleDelete = () => {
    if (!activeId) return;

    deleteCitizen(activeId, {
      onSuccess: () => {
        navigate("/citizens");
      },
    });
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="border-b border-red-200 pb-4">
        <h1 className="text-xl font-bold text-red-700">Delete Citizen</h1>
        <p className="text-xs text-slate-500 mt-1">
          Permanent removal of citizen registry and associated records.
        </p>
      </div>

      {/* Manual Search Bar */}
      <form onSubmit={handleSearch} className="flex gap-2 max-w-md">
        <input
          type="text"
          placeholder="Enter National ID to delete..."
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

      {/* Query Status States */}
      {isLoading && (
        <p className="text-xs text-slate-500">Locating citizen record...</p>
      )}

      {isError && (
        <p className="text-xs text-red-600">
          No citizen record found for National ID: [{activeId}].
        </p>
      )}

      {/* Confirmation Card */}
      {citizen && (
        <div className="max-w-lg rounded-xl border border-red-200 bg-red-50/40 p-6 space-y-4 text-xs">
          <div>
            <h2 className="text-base font-bold text-slate-900">
              {citizen.firstName} {citizen.lastName}
            </h2>
            <p className="font-mono text-slate-500">
              National ID: {citizen.nationalId}
            </p>
          </div>

          <p className="text-slate-600">
            Are you sure you want to delete this record? This action will
            permanently remove this citizen from the registry and cascade to all
            associated records.
          </p>

          <div className="flex gap-3 pt-2">
            <button
              onClick={handleDelete}
              disabled={isDeleting}
              className="rounded-lg bg-red-600 px-4 py-2 text-xs font-semibold text-white hover:bg-red-700 disabled:opacity-50"
            >
              {isDeleting ? "Deleting..." : "Confirm & Delete"}
            </button>
            <button
              onClick={() => navigate("/citizens")}
              className="rounded-lg border border-slate-300 bg-white px-4 py-2 text-xs font-medium text-slate-700 hover:bg-slate-50"
            >
              Cancel
            </button>
          </div>
        </div>
      )}
    </div>
  );
};
