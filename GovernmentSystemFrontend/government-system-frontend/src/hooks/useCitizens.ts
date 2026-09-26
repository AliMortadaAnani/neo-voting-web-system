// hooks/useCitizens.ts

// ─────────────────────────────────────────────────────────────────────────────
// IMPORTS
// ─────────────────────────────────────────────────────────────────────────────

// TanStack Query hooks:
// - useQuery:         For READ operations (GET). Fetches & caches data.
// - useMutation:      For WRITE operations (POST/PUT/DELETE). Does NOT cache.
// - useQueryClient:   Gives access to the QueryClient instance for manual cache
//                     manipulation (setQueryData, invalidateQueries, etc.).
// - keepPreviousData: A placeholderData helper that keeps the previous page's
//                     data visible while the next page is loading. Prevents
//                     the UI from flashing/emptying during pagination.
import {
  useQuery,
  useMutation,
  useQueryClient,
  keepPreviousData,
} from "@tanstack/react-query";

// Toast notifications for success/error feedback to the user.
import toast from "react-hot-toast";

// AppError is our custom error type (likely from an Axios interceptor).
// Used as the generic error type for all queries/mutations.
import type { AppError } from "../api/api";

// Domain types for Citizens.
import type {
  CitizenResponse,
  PagedResult,
  CreateCitizenDTO,
  UpdateCitizenDTO,
} from "../types/citizen.types";

// API service functions (thin wrappers around fetch/axios calls).
import {
  getCitizensPaged,
  getCitizenDetails,
  getCitizensTotalCount,
  createCitizen,
  updateCitizen,
  deleteCitizen,
} from "../services/citizen.services";

// Importing query key factories from OTHER hooks.
// This allows us to INVALIDATE their caches when a citizen changes.
// Example: If a citizen's name is updated, any cached voter/candidate data
// that references that citizen must be refetched.
import { voterKeys } from "./useVoters";
import { candidateKeys } from "./useCandidates";

// ─────────────────────────────────────────────────────────────────────────────
// 1. QUERY KEY FACTORY
// ─────────────────────────────────────────────────────────────────────────────
// WHY: TanStack Query identifies every cached piece of data by a unique "key".
//      A factory centralizes key creation so you never have typos or
//      inconsistent keys across the app.
//
// STRUCTURE:
//   all        → ["citizens"]                          (root key)
//   lists      → ["citizens", "list"]                  (all list queries)
//   list(p,s)  → ["citizens", "list", {page, pageSize}] (a specific page)
//   details    → ["citizens", "detail"]                (all detail queries)
//   detail(id) → ["citizens", "detail", nationalId]    (one specific citizen)
//   totalCount → ["citizens", "total-count"]           (the total count query)
//
// HIERARCHY MATTERS:
//   invalidateQueries({ queryKey: citizenKeys.lists() })
//   will match ALL list queries regardless of page/pageSize because
//   TanStack does a PREFIX match by default.
export const citizenKeys = {
  all: ["citizens"] as const,
  lists: () => [...citizenKeys.all, "list"] as const,
  list: (page: number, pageSize: number) =>
    [...citizenKeys.lists(), { page, pageSize }] as const,
  details: () => [...citizenKeys.all, "detail"] as const,
  detail: (nationalId: string) =>
    [...citizenKeys.details(), nationalId] as const,
  totalCount: () => [...citizenKeys.all, "total-count"] as const,
};

// ─────────────────────────────────────────────────────────────────────────────
// 2. READ HOOKS (useQuery)
// ─────────────────────────────────────────────────────────────────────────────
// useQuery returns an object with:
//   data, isLoading, isError, error, isFetching, refetch, etc.
//
// KEY OPTIONS EXPLAINED:
//   queryKey:        Unique identifier for this cache entry.
//   queryFn:         The async function that fetches data.
//   placeholderData: Shows old data while new data loads (pagination UX).
//   enabled:         Conditionally run the query (skip if false).
//   staleTime:       How long (ms) data is considered "fresh". During this
//                    window, TanStack will NOT refetch on mount/reconnect.
//   retry:           Number of retry attempts on failure (default: 3).

/**
 * Fetches a paginated list of citizens.
 * @param page     - The current page number (1-indexed).
 * @param pageSize - Number of items per page.
 *
 * RETURNS: PagedResult<CitizenResponse> which typically looks like:
 *   { items: CitizenResponse[], totalCount: number, page: number, ... }
 *
 * PAGINATION UX:
 *   `placeholderData: keepPreviousData` ensures that when the user clicks
 *   "Next Page", the old page's data remains visible until the new page
 *   arrives. Without this, `data` becomes `undefined` and the table flickers.
 */
export const useCitizens = (page = 1, pageSize = 10) => {
  return useQuery<PagedResult<CitizenResponse>, AppError>({
    queryKey: citizenKeys.list(page, pageSize),
    queryFn: () => getCitizensPaged(page, pageSize),
    placeholderData: keepPreviousData,
    staleTime: 1000 * 30, // 30 seconds
  });
};

/**
 * Fetches a single citizen's details by National ID.
 * @param nationalId - The unique national ID of the citizen.
 *
 * CONDITIONAL FETCHING (`enabled`):
 *   If `nationalId` is null/undefined/empty, the query will NOT run.
 *   This is crucial when the ID comes from a route param or a modal state
 *   that might not be set yet. Without `enabled`, the query would fire
 *   with `undefined` and likely cause a 400/404 error.
 *
 *   `Boolean(nationalId && nationalId.trim().length > 0)` ensures we also
 *   handle empty strings or whitespace-only strings gracefully.
 *
 * NOTE: We use `nationalId!` (non-null assertion) inside queryFn because
 *       `enabled` guarantees it's defined by the time the function runs.
 */
export const useCitizen = (nationalId?: string | null) => {
  return useQuery<CitizenResponse, AppError>({
    queryKey: citizenKeys.detail(nationalId ?? ""),
    queryFn: () => getCitizenDetails(nationalId!),
    enabled: Boolean(nationalId && nationalId.trim().length > 0),
    staleTime: 1000 * 60, // 1 minute
  });
};

/**
 * Fetches the TOTAL count of citizens (for dashboard stats, badges, etc.).
 *
 * WHY A SEPARATE QUERY?
 *   The total count is expensive to compute and changes less frequently
 *   than the list itself. Caching it separately for 5 minutes avoids
 *   redundant network calls every time the user navigates.
 *
 * NOTE: This query has NO pagination params, so it's a single cache entry.
 */
export const useCitizensTotalCount = () => {
  return useQuery<number, AppError>({
    queryKey: citizenKeys.totalCount(),
    queryFn: getCitizensTotalCount,
    staleTime: 1000 * 60 * 5, // 5 minutes
  });
};

// ─────────────────────────────────────────────────────────────────────────────
// 3. WRITE HOOKS (useMutation)
// ─────────────────────────────────────────────────────────────────────────────
// useMutation returns an object with:
//   mutate, mutateAsync, isPending, isError, error, reset, etc.
//
// KEY DIFFERENCES FROM useQuery:
//   - Mutations do NOT run automatically. You call `mutate()` or `mutateAsync()`.
//   - Mutations do NOT cache their results. You must manually update the cache.
//   - `onSuccess` / `onError` callbacks are where you handle cache updates.
//
// GENERICS: useMutation<TData, TError, TVariables>
//   TData:      The type returned by the mutationFn (e.g., CitizenResponse).
//   TError:     The error type (AppError).
//   TVariables: The input type passed to mutate() (e.g., CreateCitizenDTO).

export const useCitizenMutations = () => {
  // Access the QueryClient to manually manipulate the cache.
  const queryClient = useQueryClient();

  // ── HELPER: Invalidate all citizen LIST queries ──
  // `invalidateQueries` marks matching queries as "stale" and triggers a
  // background refetch if they are currently mounted/observed.
  // Because `citizenKeys.lists()` returns ["citizens", "list"], this will
  // match ALL pages and page sizes (prefix matching).
  const invalidateLists = () =>
    queryClient.invalidateQueries({ queryKey: citizenKeys.lists() });

  // ── HELPER: Invalidate the total count query ──
  const invalidateTotalCount = () =>
    queryClient.invalidateQueries({ queryKey: citizenKeys.totalCount() });

  // ───────────────────────────────────────────────────────────────────────────
  // CREATE MUTATION
  // ───────────────────────────────────────────────────────────────────────────
  // Input:  CreateCitizenDTO
  // Output: CitizenResponse
  //
  // ON SUCCESS:
  //   1. Invalidate lists     → refetch current page (new citizen appears).
  //   2. Invalidate totalCount → update dashboard stats.
  //
  // WHY NOT setQueryData HERE?
  //   For creates, it's safer to invalidate and refetch because the new
  //   item might belong on a different page (sorting/pagination).
  const createMutation = useMutation<
    CitizenResponse,
    AppError,
    CreateCitizenDTO
  >({
    mutationFn: createCitizen,
    onSuccess: () => {
      invalidateLists();
      invalidateTotalCount();
      toast.success("Citizen created successfully");
    },
    onError: (error: AppError) => {
      toast.error(error.message || "Failed to create citizen");
    },
  });

  // ───────────────────────────────────────────────────────────────────────────
  // UPDATE MUTATION
  // ───────────────────────────────────────────────────────────────────────────
  // Input:  UpdateCitizenDTO (must contain nationalId)
  // Output: CitizenResponse (the updated citizen)
  //
  // ON SUCCESS:
  //   1. setQueryData(detail) → Instantly update the single-citizen cache
  //      with the fresh data. This avoids a refetch if the user is viewing
  //      that citizen's detail page.
  //   2. Invalidate lists     → The list might show stale name/email.
  //   3. Cascade invalidate   → Voters & Candidates reference citizens.
  //      If a citizen's name changes, their voter/candidate records might
  //      display outdated info. We invalidate their entire caches.
  //
  // WHY `variables` IS AVAILABLE:
  //   The second argument to onSuccess is the variables object passed to
  //   `mutate()`. Here, `variables.nationalId` tells us WHICH detail query
  //   to update in the cache.
  const updateMutation = useMutation<
    CitizenResponse,
    AppError,
    UpdateCitizenDTO
  >({
    mutationFn: updateCitizen,
    onSuccess: (data, variables) => {
      // Instantly update the specific citizen's detail cache.
      queryClient.setQueryData(citizenKeys.detail(variables.nationalId), data);

      invalidateLists();

      // 2. Cascade: Invalidate Voter & Candidate caches
      //    Because those entities embed citizen data (name, etc.).
      queryClient.invalidateQueries({ queryKey: voterKeys.all });
      queryClient.invalidateQueries({ queryKey: candidateKeys.all });

      toast.success("Citizen updated successfully");
    },
    onError: (error: AppError) => {
      toast.error(error.message || "Failed to update citizen");
    },
  });

  // ───────────────────────────────────────────────────────────────────────────
  // DELETE MUTATION
  // ───────────────────────────────────────────────────────────────────────────
  // Input:  nationalId (string)
  // Output: boolean (success/failure)
  //
  // ON SUCCESS:
  //   1. Invalidate lists      → Remove the deleted citizen from the table.
  //   2. Invalidate totalCount → Update dashboard stats.
  //   3. removeQueries(detail) → Completely REMOVE the detail cache entry.
  //      Unlike invalidate, removeQueries deletes the cache entirely.
  //      This is important for deleted entities so that stale data doesn't
  //      linger if the user navigates back.
  //   4. Cascade invalidate    → Voters & Candidates that referenced this
  //      citizen are now orphaned or deleted (depending on backend logic).
  //      We invalidate their caches to refetch fresh data.
  //
  // WHY `_` AS FIRST PARAM?
  //   onSuccess receives (data, variables). We don't need the boolean
  //   `data` here, so we use `_` as a convention to ignore it.
  //   `deletedNationalId` is the second argument (the variables).
  const deleteMutation = useMutation<boolean, AppError, string>({
    mutationFn: deleteCitizen,
    onSuccess: (_, deletedNationalId) => {
      // 1. Invalidate Citizen queries
      invalidateLists();
      invalidateTotalCount();
      queryClient.removeQueries({
        queryKey: citizenKeys.detail(deletedNationalId),
      });

      // 2. Cascade: Invalidate Voter & Candidate caches
      queryClient.invalidateQueries({ queryKey: voterKeys.all });
      queryClient.invalidateQueries({ queryKey: candidateKeys.all });

      toast.success("Citizen deleted successfully");
    },
    onError: (error: AppError) => {
      toast.error(error.message || "Failed to delete citizen");
    },
  });

  // ───────────────────────────────────────────────────────────────────────────
  // RETURNED API
  // ───────────────────────────────────────────────────────────────────────────
  // We expose both `mutate` (fire-and-forget) and `mutateAsync` (returns a
  // Promise) for each mutation. This gives consumers flexibility:
  //
  //   - `createCitizen(data)`       → void, use onSuccess/onError callbacks.
  //   - `await createCitizenAsync(data)` → Promise, use try/catch in component.
  //
  // We also expose the loading/error states so UI can disable buttons or
  // show spinners.
  return {
    createCitizen: createMutation.mutate,
    createCitizenAsync: createMutation.mutateAsync,
    isCreating: createMutation.isPending,
    createError: createMutation.error,

    updateCitizen: updateMutation.mutate,
    updateCitizenAsync: updateMutation.mutateAsync,
    isUpdating: updateMutation.isPending,
    updateError: updateMutation.error,

    deleteCitizen: deleteMutation.mutate,
    deleteCitizenAsync: deleteMutation.mutateAsync,
    isDeleting: deleteMutation.isPending,
    deleteError: deleteMutation.error,
  };
};
