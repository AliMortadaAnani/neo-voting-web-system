import type { VoterResponse } from "../../types/voter.types";
import { GOVERNORATE_NAMES } from "../../types/citizen.types";

interface VoterCardProps {
  voter: VoterResponse;
  children?: React.ReactNode; // Optional slot for action buttons (Back, Delete, etc.)
}

export const VoterCard = ({ voter, children }: VoterCardProps) => {
  return (
    <div className="max-w-2xl rounded-xl border border-slate-200 bg-white p-6 shadow-xs space-y-5 text-xs text-slate-700">
      {/* Header: Name & Record IDs */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between border-b border-slate-100 pb-3 gap-2">
        <div>
          <h2 className="text-base font-bold text-slate-900">
            {voter.firstName} {voter.lastName}
          </h2>
          <p className="font-mono text-slate-500">
            National ID:{" "}
            <span className="font-semibold text-slate-800">
              {voter.nationalId}
            </span>
          </p>
        </div>
        <div className="flex items-center gap-2">
          <span className="rounded-md bg-slate-100 px-2.5 py-1 text-slate-600 font-mono">
            Voter ID: #{voter.id}
          </span>
          <span className="rounded-md bg-slate-100 px-2.5 py-1 text-slate-600 font-mono">
            Citizen ID: #{voter.citizenId}
          </span>
        </div>
      </div>

      {/* Demographics Grid */}
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 bg-slate-50 p-3.5 rounded-lg border border-slate-100">
        <div>
          <p className="text-slate-400 font-medium">Date of Birth</p>
          <p className="font-semibold text-slate-800 mt-0.5">
            {voter.dateOfBirth}
          </p>
        </div>
        <div>
          <p className="text-slate-400 font-medium">Governorate</p>
          <p className="font-semibold text-slate-800 mt-0.5">
            {GOVERNORATE_NAMES[voter.governorate] ?? "Unknown"} (#
            {voter.governorate})
          </p>
        </div>
        <div>
          <p className="text-slate-400 font-medium">Gender</p>
          <p className="font-semibold text-slate-800 mt-0.5">
            {voter.gender === "M" ? "Male (M)" : "Female (F)"}
          </p>
        </div>
      </div>

      {/* Cryptographic Security Fields */}
      <div className="space-y-3">
        {/* Voting Token */}
        <div>
          <label className="block text-xs font-semibold uppercase text-slate-600 mb-1">
            Voting Token
          </label>
          <div className="p-3 bg-emerald-50/50 border border-emerald-200 rounded-lg text-emerald-900 font-mono break-all select-all">
            {voter.votingToken || "No token assigned"}
          </div>
        </div>

        {/* Hashed Data */}
        <div>
          <label className="block text-xs font-semibold uppercase text-slate-600 mb-1">
            Cryptographic Hashed Data
          </label>
          <div className="p-3 bg-slate-100 border border-slate-200 rounded-lg text-slate-800 font-mono break-all select-all">
            {voter.hashedData || "No hash available"}
          </div>
        </div>
      </div>

      {/* Action Buttons Slot */}
      {children && (
        <div className="flex items-center gap-2 pt-3 border-t border-slate-100">
          {children}
        </div>
      )}
    </div>
  );
};
