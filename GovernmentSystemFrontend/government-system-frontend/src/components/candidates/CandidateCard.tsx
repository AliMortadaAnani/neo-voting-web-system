import type { CandidateResponse } from "../../types/candidate.types";
import { GOVERNORATE_NAMES } from "../../types/citizen.types";

interface CandidateCardProps {
  candidate: CandidateResponse;
  children?: React.ReactNode;
}

export const CandidateCard = ({ candidate, children }: CandidateCardProps) => {
  return (
    <div className="max-w-2xl rounded-xl border border-slate-200 bg-white p-6 shadow-xs space-y-5 text-xs text-slate-700">
      {/* Header: Name & Record IDs */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between border-b border-slate-100 pb-3 gap-2">
        <div>
          <h2 className="text-base font-bold text-slate-900">
            {candidate.firstName} {candidate.lastName}
          </h2>
          <p className="font-mono text-slate-500">
            National ID:{" "}
            <span className="font-semibold text-slate-800">
              {candidate.nationalId}
            </span>
          </p>
        </div>
        <div className="flex items-center gap-2">
          <span className="rounded-md bg-purple-50 text-purple-700 border border-purple-200 px-2.5 py-1 font-mono font-medium">
            Candidate ID: #{candidate.id}
          </span>
          <span className="rounded-md bg-slate-100 px-2.5 py-1 text-slate-600 font-mono">
            Citizen ID: #{candidate.citizenId}
          </span>
        </div>
      </div>

      {/* Demographics Grid */}
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 bg-slate-50 p-3.5 rounded-lg border border-slate-100">
        <div>
          <p className="text-slate-400 font-medium">Date of Birth</p>
          <p className="font-semibold text-slate-800 mt-0.5">
            {candidate.dateOfBirth}
          </p>
        </div>
        <div>
          <p className="text-slate-400 font-medium">Governorate</p>
          <p className="font-semibold text-slate-800 mt-0.5">
            {GOVERNORATE_NAMES[candidate.governorate] ?? "Unknown"} (#
            {candidate.governorate})
          </p>
        </div>
        <div>
          <p className="text-slate-400 font-medium">Gender</p>
          <p className="font-semibold text-slate-800 mt-0.5">
            {candidate.gender === "M" ? "Male (M)" : "Female (F)"}
          </p>
        </div>
      </div>

      {/* Cryptographic Security Fields */}
      <div className="space-y-3">
        {/* Nomination Token */}
        <div>
          <label className="block text-xs font-semibold uppercase text-slate-600 mb-1">
            Nomination Token
          </label>
          <div className="p-3 bg-purple-50/50 border border-purple-200 rounded-lg text-purple-900 font-mono break-all select-all">
            {candidate.nominationToken || "No token assigned"}
          </div>
        </div>

        {/* Hashed Data */}
        <div>
          <label className="block text-xs font-semibold uppercase text-slate-600 mb-1">
            Cryptographic Hashed Data
          </label>
          <div className="p-3 bg-slate-100 border border-slate-200 rounded-lg text-slate-800 font-mono break-all select-all">
            {candidate.hashedData || "No hash available"}
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
