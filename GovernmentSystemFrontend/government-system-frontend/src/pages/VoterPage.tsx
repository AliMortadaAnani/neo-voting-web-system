import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { voterFormSchema } from "../schemas/voter.schemas";
import type { VoterRequestDTO } from "../types/voter.types";
import {
  useVoters,
  useVoter,
  useVotersTotalCount,
  useVoterMutations,
} from "../hooks/useVoters";
import { GOVERNORATE_NAMES } from "../types/citizen.types";

export function VoterPage() {
  const [page, setPage] = useState(1);
  const pageSize = 5;

  const { data: pagedData, isLoading, isError } = useVoters(page, pageSize);
  const { data: totalCount } = useVotersTotalCount();

  const [fetchQueryDto, setFetchQueryDto] = useState<VoterRequestDTO | null>(
    null,
  );
  const { data: selectedVoter, isLoading: isFetchingDetails } =
    useVoter(fetchQueryDto);

  const {
    addVoter,
    regenerateVotingToken,
    deleteVoter,
    isAddingVoter,
    isRegeneratingVotingToken,
    isDeletingVoter,
  } = useVoterMutations();

  const addForm = useForm<VoterRequestDTO>({
    resolver: zodResolver(voterFormSchema),
    defaultValues: { nationalId: "" },
  });

  const fetchForm = useForm<VoterRequestDTO>({
    resolver: zodResolver(voterFormSchema),
    defaultValues: { nationalId: "" },
  });

  const tokenForm = useForm<VoterRequestDTO>({
    resolver: zodResolver(voterFormSchema),
    defaultValues: { nationalId: "" },
  });

  const deleteForm = useForm<VoterRequestDTO>({
    resolver: zodResolver(voterFormSchema),
    defaultValues: { nationalId: "" },
  });

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-2xl font-bold tracking-tight">Voter Registry</h1>
        <p className="text-sm text-gray-500">
          Total Registered Voters: {totalCount ?? 0}
        </p>
      </div>

      <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
        {/* Enroll Voter */}
        <div className="rounded-xl border bg-white p-6 shadow-sm dark:border-zinc-800 dark:bg-zinc-900">
          <h3 className="mb-4 text-lg font-semibold">
            Enroll Citizen as Voter
          </h3>
          <form
            onSubmit={addForm.handleSubmit((data) => addVoter(data))}
            className="space-y-4"
          >
            <div>
              <label className="block text-xs font-medium">National ID</label>
              <input
                {...addForm.register("nationalId")}
                className="w-full rounded border px-3 py-1.5 text-sm dark:bg-zinc-800 dark:border-zinc-700"
              />
              <p className="text-xs text-red-500">
                {addForm.formState.errors.nationalId?.message}
              </p>
            </div>
            <button
              type="submit"
              disabled={isAddingVoter}
              className="w-full rounded bg-blue-600 py-2 text-sm font-medium text-white hover:bg-blue-700"
            >
              {isAddingVoter ? "Enrolling..." : "Enroll Voter"}
            </button>
          </form>
        </div>

        {/* Regenerate Token Form */}
        <div className="rounded-xl border bg-white p-6 shadow-sm dark:border-zinc-800 dark:bg-zinc-900">
          <h3 className="mb-4 text-lg font-semibold">
            Regenerate Voting Token
          </h3>
          <form
            onSubmit={tokenForm.handleSubmit((data) =>
              regenerateVotingToken(data),
            )}
            className="space-y-4"
          >
            <div>
              <label className="block text-xs font-medium">National ID</label>
              <input
                {...tokenForm.register("nationalId")}
                className="w-full rounded border px-3 py-1.5 text-sm dark:bg-zinc-800 dark:border-zinc-700"
              />
            </div>
            <button
              type="submit"
              disabled={isRegeneratingVotingToken}
              className="w-full rounded bg-purple-600 py-2 text-sm font-medium text-white hover:bg-purple-700"
            >
              {isRegeneratingVotingToken
                ? "Regenerating..."
                : "Generate New Token"}
            </button>
          </form>
        </div>
      </div>

      <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
        {/* Fetch Voter Details */}
        <div className="rounded-xl border bg-white p-6 shadow-sm dark:border-zinc-800 dark:bg-zinc-900">
          <h3 className="mb-4 text-lg font-semibold">Inspect Voter Details</h3>
          <form
            onSubmit={fetchForm.handleSubmit((data) => setFetchQueryDto(data))}
            className="flex gap-2"
          >
            <input
              placeholder="National ID"
              {...fetchForm.register("nationalId")}
              className="flex-1 rounded border px-3 py-1.5 text-sm dark:bg-zinc-800 dark:border-zinc-700"
            />
            <button
              type="submit"
              className="rounded bg-gray-800 px-4 py-1.5 text-sm text-white dark:bg-zinc-700"
            >
              Fetch
            </button>
          </form>
          {isFetchingDetails && (
            <p className="mt-2 text-xs text-gray-400">Loading details...</p>
          )}
          {selectedVoter && (
            <div className="mt-4 rounded bg-gray-50 p-3 text-xs dark:bg-zinc-800">
              <p>
                <strong>Name:</strong> {selectedVoter.firstName}{" "}
                {selectedVoter.lastName}
              </p>
              <p>
                <strong>Voting Token:</strong>{" "}
                <span className="font-mono text-purple-500">
                  {selectedVoter.votingToken}
                </span>
              </p>
              <p>
                <strong>Governorate:</strong>{" "}
                {GOVERNORATE_NAMES[selectedVoter.governorate]}
              </p>
            </div>
          )}
        </div>

        {/* Delete Voter */}
        <div className="rounded-xl border bg-white p-6 shadow-sm dark:border-zinc-800 dark:bg-zinc-900">
          <h3 className="mb-4 text-lg font-semibold text-red-600">
            Remove Voter Status
          </h3>
          <form
            onSubmit={deleteForm.handleSubmit((data) => deleteVoter(data))}
            className="flex gap-2"
          >
            <input
              placeholder="National ID to remove"
              {...deleteForm.register("nationalId")}
              className="flex-1 rounded border px-3 py-1.5 text-sm dark:bg-zinc-800 dark:border-zinc-700"
            />
            <button
              type="submit"
              disabled={isDeletingVoter}
              className="rounded bg-red-600 px-4 py-1.5 text-sm text-white hover:bg-red-700"
            >
              {isDeletingVoter ? "Removing..." : "Remove"}
            </button>
          </form>
        </div>
      </div>

      {/* Paged Table List */}
      <div className="rounded-xl border bg-white p-6 shadow-sm dark:border-zinc-800 dark:bg-zinc-900">
        <h3 className="mb-4 text-lg font-semibold">Voters Registry (Paged)</h3>
        {isLoading && <p>Loading voters...</p>}
        {isError && <p className="text-red-500">Failed to load voters list.</p>}
        {pagedData && (
          <div className="space-y-4">
            <table className="w-full text-left text-sm">
              <thead className="border-b text-xs uppercase text-gray-400 dark:border-zinc-800">
                <tr>
                  <th className="pb-3">National ID</th>
                  <th className="pb-3">Full Name</th>
                  <th className="pb-3">Voting Token</th>
                  <th className="pb-3">Governorate</th>
                </tr>
              </thead>
              <tbody className="divide-y dark:divide-zinc-800">
                {pagedData.data.map((v) => (
                  <tr key={v.id}>
                    <td className="py-3 font-mono">{v.nationalId}</td>
                    <td className="py-3">
                      {v.firstName} {v.lastName}
                    </td>
                    <td className="py-3 font-mono text-purple-500">
                      {v.votingToken}
                    </td>
                    <td className="py-3">{GOVERNORATE_NAMES[v.governorate]}</td>
                  </tr>
                ))}
              </tbody>
            </table>
            <div className="flex items-center justify-between pt-2">
              <button
                disabled={!pagedData.hasPreviousPage}
                onClick={() => setPage((p) => p - 1)}
                className="rounded border px-3 py-1 text-xs disabled:opacity-30 dark:border-zinc-700"
              >
                Previous
              </button>
              <span className="text-xs text-gray-500">
                Page {pagedData.currentPage} of {pagedData.totalPages}
              </span>
              <button
                disabled={!pagedData.hasNextPage}
                onClick={() => setPage((p) => p + 1)}
                className="rounded border px-3 py-1 text-xs disabled:opacity-30 dark:border-zinc-700"
              >
                Next
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
