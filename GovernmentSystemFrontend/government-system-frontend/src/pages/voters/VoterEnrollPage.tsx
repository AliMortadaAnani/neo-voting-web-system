import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useVoterMutations } from "../../hooks/useVoters";
import { VoterCard } from "../../components/voters/VoterCard";
import type { VoterResponse } from "../../types/voter.types";

export const VoterEnrollPage = () => {
  const navigate = useNavigate();
  const [nationalId, setNationalId] = useState("");
  const [enrolledVoter, setEnrolledVoter] = useState<VoterResponse | null>(
    null,
  );

  const { addVoter, isAdding } = useVoterMutations();

  const handleEnroll = (e: React.FormEvent) => {
    e.preventDefault();
    const trimmed = nationalId.trim();
    if (!trimmed) return;

    addVoter(
      { nationalId: trimmed },
      {
        onSuccess: (data) => {
          // Immediately display the newly created card with the new token
          setEnrolledVoter(data);
          setNationalId("");
        },
      },
    );
  };

  return (
    <div className="space-y-6">
      <div className="border-b border-slate-200 pb-4">
        <h1 className="text-xl font-bold text-slate-900">
          Enroll Citizen as Voter
        </h1>
        <p className="text-xs text-slate-500 mt-1">
          Enter an existing Citizen's National ID to register them and generate
          their voting token.
        </p>
      </div>

      {/* Single Input Form */}
      <form onSubmit={handleEnroll} className="flex gap-2 max-w-md">
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
          {isAdding ? "Enrolling..." : "Enroll Voter"}
        </button>
      </form>

      {/* Display Card on Success */}
      {enrolledVoter && (
        <div className="space-y-3 pt-2">
          <p className="text-xs font-semibold text-emerald-700">
            ✓ Voter successfully registered! Newly issued voting credentials:
          </p>
          <VoterCard voter={enrolledVoter}>
            <button
              onClick={() => navigate("/voters")}
              className="rounded bg-slate-900 px-3 py-1.5 text-xs font-semibold text-white hover:bg-slate-800"
            >
              Back to Voters List
            </button>
            <button
              onClick={() => setEnrolledVoter(null)}
              className="rounded border border-slate-300 px-3 py-1.5 text-xs text-slate-600 hover:bg-slate-50"
            >
              Enroll Another
            </button>
          </VoterCard>
        </div>
      )}
    </div>
  );
};
