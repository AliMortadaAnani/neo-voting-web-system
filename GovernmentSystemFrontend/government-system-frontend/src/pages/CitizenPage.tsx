import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  createCitizenFormSchema,
  updateCitizenFormSchema,
  fetchCitizenFormSchema,
} from "../schemas/citizen.schemas";
import type {
  CitizenCreateRequestDTO,
  CitizenUpdateRequestDTO,
  CitizenFetchRequestDTO,
} from "../types/citizen.types";
import { GOVERNORATE_NAMES } from "../types/citizen.types";
import {
  useCitizens,
  useCitizen,
  useCitizensTotalCount,
  useCitizenMutations,
} from "../hooks/useCitizens";

export function CitizenPage() {
  const [page, setPage] = useState(1);
  const pageSize = 5;

  // Query states
  const { data: pagedData, isLoading, isError } = useCitizens(page, pageSize);
  const { data: totalCount } = useCitizensTotalCount();

  // Fetch single citizen details state
  const [fetchQueryDto, setFetchQueryDto] =
    useState<CitizenFetchRequestDTO | null>(null);
  const { data: selectedCitizen, isLoading: isFetchingDetails } =
    useCitizen(fetchQueryDto);

  // Mutations
  const {
    createCitizen,
    updateCitizen,
    deleteCitizen,
    isCreatingCitizen,
    isUpdatingCitizen,
    isDeletingCitizen,
  } = useCitizenMutations();

  // Forms
  const createForm = useForm<CitizenCreateRequestDTO>({
    resolver: zodResolver(createCitizenFormSchema),
    defaultValues: {
      firstName: "",
      lastName: "",
      dateOfBirth: "",
      governorate: 1,
      gender: "M",
    },
  });

  const updateForm = useForm<CitizenUpdateRequestDTO>({
    resolver: zodResolver(updateCitizenFormSchema),
    defaultValues: {
      nationalId: "",
      firstName: "",
      lastName: "",
      dateOfBirth: "",
      governorate: 1,
      gender: "M",
    },
  });

  const fetchForm = useForm<CitizenFetchRequestDTO>({
    resolver: zodResolver(fetchCitizenFormSchema),
    defaultValues: { nationalId: "" },
  });

  const deleteForm = useForm<CitizenFetchRequestDTO>({
    resolver: zodResolver(fetchCitizenFormSchema),
    defaultValues: { nationalId: "" },
  });

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-2xl font-bold tracking-tight">
          Citizen Management
        </h1>
        <p className="text-sm text-gray-500">
          Total System Citizens: {totalCount ?? 0}
        </p>
      </div>

      {/* Grid wrapper for Actions */}
      <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
        {/* 1. Create Citizen */}
        <div className="rounded-xl border bg-white p-6 shadow-sm dark:border-zinc-800 dark:bg-zinc-900">
          <h3 className="mb-4 text-lg font-semibold">Create Citizen</h3>
          <form
            onSubmit={createForm.handleSubmit((data) => createCitizen(data))}
            className="space-y-4"
          >
            <div>
              <label className="block text-xs font-medium">First Name</label>
              <input
                {...createForm.register("firstName")}
                className="w-full rounded border px-3 py-1.5 text-sm dark:bg-zinc-800 dark:border-zinc-700"
              />
              <p className="text-xs text-red-500">
                {createForm.formState.errors.firstName?.message}
              </p>
            </div>
            <div>
              <label className="block text-xs font-medium">Last Name</label>
              <input
                {...createForm.register("lastName")}
                className="w-full rounded border px-3 py-1.5 text-sm dark:bg-zinc-800 dark:border-zinc-700"
              />
              <p className="text-xs text-red-500">
                {createForm.formState.errors.lastName?.message}
              </p>
            </div>
            <div>
              <label className="block text-xs font-medium">
                Date of Birth (18+)
              </label>
              <input
                type="date"
                {...createForm.register("dateOfBirth")}
                className="w-full rounded border px-3 py-1.5 text-sm dark:bg-zinc-800 dark:border-zinc-700"
              />
              <p className="text-xs text-red-500">
                {createForm.formState.errors.dateOfBirth?.message}
              </p>
            </div>
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-xs font-medium">
                  Governorate (1-5)
                </label>
                <input
                  type="number"
                  {...createForm.register("governorate", {
                    valueAsNumber: true,
                  })}
                  className="w-full rounded border px-3 py-1.5 text-sm dark:bg-zinc-800 dark:border-zinc-700"
                />
              </div>
              <div>
                <label className="block text-xs font-medium">
                  Gender (M/F)
                </label>
                <input
                  {...createForm.register("gender")}
                  className="w-full rounded border px-3 py-1.5 text-sm dark:bg-zinc-800 dark:border-zinc-700"
                />
              </div>
            </div>
            <button
              type="submit"
              disabled={isCreatingCitizen}
              className="w-full rounded bg-blue-600 py-2 text-sm font-medium text-white hover:bg-blue-700"
            >
              {isCreatingCitizen ? "Creating..." : "Create Citizen"}
            </button>
          </form>
        </div>

        {/* 2. Update & Fetch Details & Delete Container */}
        <div className="space-y-6">
          {/* Fetch Details Form */}
          <div className="rounded-xl border bg-white p-6 shadow-sm dark:border-zinc-800 dark:bg-zinc-900">
            <h3 className="mb-4 text-lg font-semibold">
              Inspect Citizen Details
            </h3>
            <form
              onSubmit={fetchForm.handleSubmit((data) =>
                setFetchQueryDto(data),
              )}
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
            {selectedCitizen && (
              <div className="mt-4 rounded bg-gray-50 p-3 text-xs dark:bg-zinc-800">
                <p>
                  <strong>Name:</strong> {selectedCitizen.firstName}{" "}
                  {selectedCitizen.lastName}
                </p>
                <p>
                  <strong>DOB:</strong> {selectedCitizen.dateOfBirth} |{" "}
                  <strong>Gender:</strong> {selectedCitizen.gender}
                </p>
                <p>
                  <strong>Governorate:</strong>{" "}
                  {GOVERNORATE_NAMES[selectedCitizen.governorate]}
                </p>
              </div>
            )}
          </div>

          {/* Delete Form */}
          <div className="rounded-xl border bg-white p-6 shadow-sm dark:border-zinc-800 dark:bg-zinc-900">
            <h3 className="mb-4 text-lg font-semibold text-red-600">
              Delete Citizen
            </h3>
            <form
              onSubmit={deleteForm.handleSubmit((data) => deleteCitizen(data))}
              className="flex gap-2"
            >
              <input
                placeholder="National ID to delete"
                {...deleteForm.register("nationalId")}
                className="flex-1 rounded border px-3 py-1.5 text-sm dark:bg-zinc-800 dark:border-zinc-700"
              />
              <button
                type="submit"
                disabled={isDeletingCitizen}
                className="rounded bg-red-600 px-4 py-1.5 text-sm text-white hover:bg-red-700"
              >
                {isDeletingCitizen ? "Deleting..." : "Delete"}
              </button>
            </form>
          </div>
        </div>
      </div>

      {/* Update Section */}
      <div className="rounded-xl border bg-white p-6 shadow-sm dark:border-zinc-800 dark:bg-zinc-900">
        <h3 className="mb-4 text-lg font-semibold">Update Citizen Details</h3>
        <form
          onSubmit={updateForm.handleSubmit((data) => updateCitizen(data))}
          className="grid grid-cols-1 gap-4 sm:grid-cols-3"
        >
          <div>
            <label className="block text-xs font-medium">
              Target National ID
            </label>
            <input
              {...updateForm.register("nationalId")}
              className="w-full rounded border px-3 py-1.5 text-sm dark:bg-zinc-800 dark:border-zinc-700"
            />
          </div>
          <div>
            <label className="block text-xs font-medium">New First Name</label>
            <input
              {...updateForm.register("firstName")}
              className="w-full rounded border px-3 py-1.5 text-sm dark:bg-zinc-800 dark:border-zinc-700"
            />
          </div>
          <div>
            <label className="block text-xs font-medium">New Last Name</label>
            <input
              {...updateForm.register("lastName")}
              className="w-full rounded border px-3 py-1.5 text-sm dark:bg-zinc-800 dark:border-zinc-700"
            />
          </div>
          <div>
            <label className="block text-xs font-medium">Date of Birth</label>
            <input
              type="date"
              {...updateForm.register("dateOfBirth")}
              className="w-full rounded border px-3 py-1.5 text-sm dark:bg-zinc-800 dark:border-zinc-700"
            />
          </div>
          <div>
            <label className="block text-xs font-medium">
              Governorate (1-5)
            </label>
            <input
              type="number"
              {...updateForm.register("governorate", { valueAsNumber: true })}
              className="w-full rounded border px-3 py-1.5 text-sm dark:bg-zinc-800 dark:border-zinc-700"
            />
          </div>
          <div>
            <label className="block text-xs font-medium">Gender (M/F)</label>
            <input
              {...updateForm.register("gender")}
              className="w-full rounded border px-3 py-1.5 text-sm dark:bg-zinc-800 dark:border-zinc-700"
            />
          </div>
          <div className="sm:col-span-3">
            <button
              type="submit"
              disabled={isUpdatingCitizen}
              className="rounded bg-amber-600 px-6 py-2 text-sm font-medium text-white hover:bg-amber-700"
            >
              {isUpdatingCitizen ? "Updating..." : "Save Changes"}
            </button>
          </div>
        </form>
      </div>

      {/* Paged Table List */}
      <div className="rounded-xl border bg-white p-6 shadow-sm dark:border-zinc-800 dark:bg-zinc-900">
        <h3 className="mb-4 text-lg font-semibold">
          Citizens Directory (Paged)
        </h3>
        {isLoading && <p>Loading registry...</p>}
        {isError && (
          <p className="text-red-500">Failed to load citizens registry.</p>
        )}
        {pagedData && (
          <div className="space-y-4">
            <table className="w-full text-left text-sm">
              <thead className="border-b text-xs uppercase text-gray-400 dark:border-zinc-800">
                <tr>
                  <th className="pb-3">National ID</th>
                  <th className="pb-3">Full Name</th>
                  <th className="pb-3">DOB</th>
                  <th className="pb-3">Governorate</th>
                  <th className="pb-3">Gender</th>
                </tr>
              </thead>
              <tbody className="divide-y dark:divide-zinc-800">
                {pagedData.data.map((c) => (
                  <tr key={c.id}>
                    <td className="py-3 font-mono">{c.nationalId}</td>
                    <td className="py-3">
                      {c.firstName} {c.lastName}
                    </td>
                    <td className="py-3">{c.dateOfBirth}</td>
                    <td className="py-3">{GOVERNORATE_NAMES[c.governorate]}</td>
                    <td className="py-3">{c.gender}</td>
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
