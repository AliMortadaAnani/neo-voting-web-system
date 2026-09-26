import { useState } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { useCitizen, useCitizenMutations } from "../../hooks/useCitizens";
import { CitizenForm } from "../../components/citizens/CitizenForm";
import type { CitizenFormValues } from "../../schemas/citizen.schemas";

export const CitizenEditPage = () => {
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
  const { updateCitizen, isUpdating } = useCitizenMutations();

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    const trimmedId = lookupId.trim();

    if (trimmedId) {
      setActiveId(trimmedId);

      // Keep location.state synced so browser refresh (F5) keeps the form open
      navigate("/citizens/edit", {
        state: { nationalId: trimmedId },
        replace: true,
      });
    }
  };

  const handleClear = () => {
    setLookupId("");
    setActiveId("");
    navigate("/citizens/edit", { state: null, replace: true });
  };

  const handleUpdate = (formData: CitizenFormValues) => {
    if (!activeId) return;

    updateCitizen(
      {
        nationalId: activeId,
        ...formData,
        governorate: Number(formData.governorate),
      },
      {
        onSuccess: () => {
          navigate("/citizens");
        },
      },
    );
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="border-b border-slate-200 pb-4">
        <h1 className="text-xl font-bold text-slate-900">
          Update Citizen Details
        </h1>
        <p className="text-xs text-slate-500 mt-1">
          Search for a citizen by National ID to modify their record securely.
        </p>
      </div>

      {/* Manual Search Bar */}
      <form onSubmit={handleSearch} className="flex gap-2 max-w-md">
        <input
          type="text"
          placeholder="Enter National ID to edit..."
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
        <p className="text-xs text-slate-500">Loading citizen data...</p>
      )}

      {isError && (
        <p className="text-xs text-red-600">
          No citizen record found for National ID: [{activeId}].
        </p>
      )}

      {/* Populated Reusable Form */}
      {citizen && (
        <CitizenForm
          key={citizen.nationalId}
          nationalId={citizen.nationalId}
          initialValues={{
            firstName: citizen.firstName,
            lastName: citizen.lastName,
            dateOfBirth: citizen.dateOfBirth,
            governorate: String(citizen.governorate),
            gender: citizen.gender,
          }}
          onSubmit={handleUpdate}
          isSubmitting={isUpdating}
          submitLabel="Update Citizen"
          onCancel={() => navigate("/citizens")}
        />
      )}
    </div>
  );
};
