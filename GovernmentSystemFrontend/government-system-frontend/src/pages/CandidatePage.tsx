import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { candidateFormSchema } from "../schemas/candidate.schemas";
import type { CandidateRequestDTO } from "../types/candidate.types";
import {
  useCandidates,
  useCandidate,
  useCandidatesTotalCount,
  useCandidateMutations,
} from "../hooks/useCandidates";
import { GOVERNORATE_NAMES } from "../types/citizen.types";

export function CandidatePage() {
  const [page, setPage] = useState(1);
  const pageSize = 5;

  const { data: pagedData, isLoading, isError } = useCandidates(page, pageSize);
  const { data: totalCount } = useCandidatesTotalCount();

  const [fetchQueryDto, setFetchQueryDto] =
    useState<CandidateRequestDTO | null>(null);
  const { data: selectedCandidate, isLoading: isFetchingDetails } =
    useCandidate(fetchQueryDto);

  const {
    addCandidate,
    regenerateToken,
    deleteCandidate,
    isAdding,
    isRegeneratingToken,
    isDeleting,
  } = useCandidateMutations();

  const addForm = useForm<CandidateRequestDTO>({
    resolver: zodResolver(candidateFormSchema),
    defaultValues: { nationalId: "" },
  });

  const fetchForm = useForm<CandidateRequestDTO>({
    resolver: zodResolver(candidateFormSchema),
    defaultValues: { nationalId: "" },
  });

  const tokenForm = useForm<CandidateRequestDTO>({
    resolver: zodResolver(candidateFormSchema),
    defaultValues: { nationalId: "" },
  });

  const deleteForm = useForm<CandidateRequestDTO>({
    resolver: zodResolver(candidateFormSchema),
    defaultValues: { nationalId: "" },
  });

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-2xl font-bold tracking-tight">
          Candidate Registry
        </h1>
        <p className="text-sm text-gray-500">
          Total Registered Candidates: {totalCount ?? 0}
        </p>
      </div>

      <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
        {/* Nominate Candidate */}
        <div className="rounded-xl border bg-white p-6 shadow-sm dark:border-zinc-800 dark:bg-zinc-900">
          <h3 className="mb-4 text-lg font-semibold">
            Nominate Citizen as Candidate
          </h3>
          <form
            onSubmit={addForm.handleSubmit((data) => addCandidate(data))}
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
              disabled={isAdding}
              className="w-full rounded bg-blue-600 py-2 text-sm font-medium text-white hover:bg-blue-700"
            >
              {isAdding ? "Nominating..." : "Nominate Candidate"}
            </button>
          </form>
        </div>

        {/* Regenerate Token Form */}
        <div className="rounded-xl border bg-white p-6 shadow-sm dark:border-zinc-800 dark:bg-zinc-900">
          <h3 className="mb-4 text-lg font-semibold">
            Regenerate Nomination Token
          </h3>
          <form
            onSubmit={tokenForm.handleSubmit((data) => regenerateToken(data))}
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
              disabled={isRegeneratingToken}
              className="w-full rounded bg-purple-600 py-2 text-sm font-medium text-white hover:bg-purple-700"
            >
              {isRegeneratingToken ? "Regenerating..." : "Generate New Token"}
            </button>
          </form>
        </div>
      </div>

      <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
        {/* Fetch Candidate Details */}
        <div className="rounded-xl border bg-white p-6 shadow-sm dark:border-zinc-800 dark:bg-zinc-900">
          <h3 className="mb-4 text-lg font-semibold">
            Inspect Candidate Details
          </h3>
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
          {selectedCandidate && (
            <div className="mt-4 rounded bg-gray-50 p-3 text-xs dark:bg-zinc-800">
              <p>
                <strong>Name:</strong> {selectedCandidate.firstName}{" "}
                {selectedCandidate.lastName}
              </p>
              <p>
                <strong>Nomination Token:</strong>{" "}
                <span className="font-mono text-purple-500">
                  {selectedCandidate.nominationToken}
                </span>
              </p>
              <p>
                <strong>Governorate:</strong>{" "}
                {GOVERNORATE_NAMES[selectedCandidate.governorate]}
              </p>
            </div>
          )}
        </div>

        {/* Delete Candidate */}
        <div className="rounded-xl border bg-white p-6 shadow-sm dark:border-zinc-800 dark:bg-zinc-900">
          <h3 className="mb-4 text-lg font-semibold text-red-600">
            Remove Candidate Status
          </h3>
          <form
            onSubmit={deleteForm.handleSubmit((data) => deleteCandidate(data))}
            className="flex gap-2"
          >
            <input
              placeholder="National ID to remove"
              {...deleteForm.register("nationalId")}
              className="flex-1 rounded border px-3 py-1.5 text-sm dark:bg-zinc-800 dark:border-zinc-700"
            />
            <button
              type="submit"
              disabled={isDeleting}
              className="rounded bg-red-600 px-4 py-1.5 text-sm text-white hover:bg-red-700"
            >
              {isDeleting ? "Removing..." : "Remove"}
            </button>
          </form>
        </div>
      </div>

      {/* Paged Table List */}
      <div className="rounded-xl border bg-white p-6 shadow-sm dark:border-zinc-800 dark:bg-zinc-900">
        <h3 className="mb-4 text-lg font-semibold">
          Candidates Registry (Paged)
        </h3>
        {isLoading && <p>Loading candidates...</p>}
        {isError && (
          <p className="text-red-500">Failed to load candidates list.</p>
        )}
        {pagedData && (
          <div className="space-y-4">
            <table className="w-full text-left text-sm">
              <thead className="border-b text-xs uppercase text-gray-400 dark:border-zinc-800">
                <tr>
                  <th className="pb-3">National ID</th>
                  <th className="pb-3">Full Name</th>
                  <th className="pb-3">Nomination Token</th>
                  <th className="pb-3">Governorate</th>
                </tr>
              </thead>
              <tbody className="divide-y dark:divide-zinc-800">
                {pagedData.data.map((c) => (
                  <tr key={c.id}>
                    <td className="py-3 font-mono">{c.nationalId}</td>
                    <td className="py-3">
                      {c.firstName} {c.lastName}
                    </td>
                    <td className="py-3 font-mono text-purple-500">
                      {c.nominationToken}
                    </td>
                    <td className="py-3">{GOVERNORATE_NAMES[c.governorate]}</td>
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
