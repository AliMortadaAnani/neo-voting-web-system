import { useState } from "react";
import { useNavigate, Link, useLocation } from "react-router-dom";
import { useCitizen } from "../../hooks/useCitizens";
import { GOVERNORATE_NAMES } from "../../types/citizen.types";
export const CitizenDetailsPage = () => {
  const navigate = useNavigate();
  const location = useLocation();

  // 1. Read National ID safely from hidden navigation state (no ID in URL)
  const state = location.state as { nationalId?: string } | null;
  const initialId = state?.nationalId ?? "";

  // 2. Local input state vs. Active search query state
  const [lookupId, setLookupId] = useState<string>(initialId);
  const [activeId, setActiveId] = useState<string>(initialId);

  // 3. React Query: automatically runs if activeId is not empty
  const { data: citizen, isLoading, isError } = useCitizen(activeId);

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    const trimmedId = lookupId.trim();

    if (trimmedId) {
      setActiveId(trimmedId);

      // Keep location.state in sync with what was typed so browser refresh (F5) remembers it
      navigate("/citizens/details", {
        state: { nationalId: trimmedId },
        replace: true,
      });
    }
  };

  const handleClear = () => {
    setLookupId("");
    setActiveId("");
    navigate("/citizens/details", { state: null, replace: true });
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="border-b border-slate-200 pb-4">
        <h1 className="text-xl font-bold text-slate-900">Citizen Profile</h1>
        <p className="text-xs text-slate-500 mt-1">
          Inspect personal record details securely.
        </p>
      </div>

      {/* Manual Search Bar */}
      <form onSubmit={handleSearch} className="flex gap-2 max-w-md">
        <input
          type="text"
          placeholder="Enter National ID to inspect..."
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
        <p className="text-xs text-slate-500">Loading citizen profile...</p>
      )}

      {isError && (
        <p className="text-xs text-red-600">
          No citizen record found for National ID: [{activeId}].
        </p>
      )}

      {/* Citizen Details Card */}
      {citizen && (
        <div className="max-w-lg rounded-xl border border-slate-200 bg-white p-6 shadow-xs space-y-4 text-xs">
          <div className="flex justify-between items-start border-b border-slate-100 pb-3">
            <div>
              <h2 className="text-base font-bold text-slate-900">
                {citizen.firstName} {citizen.lastName}
              </h2>
              <p className="font-mono text-slate-500">
                ID: {citizen.nationalId}
              </p>
            </div>
            <span className="rounded-full bg-slate-100 px-2.5 py-1 text-slate-700 font-medium">
              {GOVERNORATE_NAMES[citizen.governorate] ?? "Unknown"}
            </span>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <p className="text-slate-400 font-medium">Date of Birth</p>
              <p className="font-semibold text-slate-800">
                {citizen.dateOfBirth}
              </p>
            </div>
            <div>
              <p className="text-slate-400 font-medium">Gender</p>
              <p className="font-semibold text-slate-800">
                {citizen.gender === "M" ? "Male" : "Female"}
              </p>
            </div>
          </div>

          {/* Action Links (All use hidden state, 0 IDs in URLs) */}
          <div className="flex gap-2 pt-4 border-t border-slate-100">
            <Link
              to="/citizens/edit"
              state={{ nationalId: citizen.nationalId }}
              className="rounded bg-slate-900 px-3 py-1.5 text-xs font-semibold text-white hover:bg-slate-800"
            >
              Edit Citizen
            </Link>

            <Link
              to="/citizens/delete"
              state={{ nationalId: citizen.nationalId }}
              className="rounded border border-red-200 px-3 py-1.5 text-xs font-semibold text-red-600 hover:bg-red-50"
            >
              Delete
            </Link>

            <button
              onClick={() => navigate("/citizens")}
              className="rounded border border-slate-200 px-3 py-1.5 text-xs text-slate-600 hover:bg-slate-50"
            >
              Back to List
            </button>
          </div>
        </div>
      )}
    </div>
  );
};
