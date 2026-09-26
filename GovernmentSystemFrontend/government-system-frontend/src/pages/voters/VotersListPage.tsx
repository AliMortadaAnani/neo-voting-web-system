import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useVoters, useVotersTotalCount } from "../../hooks/useVoters";
import { GOVERNORATE_NAMES } from "../../types/citizen.types";

export const VotersListPage = () => {
  const navigate = useNavigate();
  const [page, setPage] = useState<number>(1);
  const [pageSize, setPageSize] = useState<number>(10);

  const {
    data: pagedData,
    isLoading,
    isPlaceholderData,
  } = useVoters(page, pageSize);
  const { data: totalCount, isLoading: isCountLoading } = useVotersTotalCount();

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 border-b border-slate-200 pb-4">
        <div>
          <h1 className="text-xl font-bold text-slate-900">
            Registered Voters
          </h1>
          <p className="text-xs text-slate-500 mt-1">
            Total Enrolled:{" "}
            <span className="font-semibold text-slate-800">
              {isCountLoading ? "..." : (totalCount ?? "0")}
            </span>
          </p>
        </div>

        <Link
          to="/voters/create"
          className="rounded-lg bg-slate-900 px-4 py-2 text-xs font-semibold text-white hover:bg-slate-800"
        >
          + Enroll Voter
        </Link>
      </div>

      {/* Page Size Toolbar */}
      <div className="flex items-center justify-between text-xs text-slate-600">
        <div className="flex items-center gap-2">
          <span>Rows per page:</span>
          <select
            value={pageSize}
            onChange={(e) => {
              setPageSize(Number(e.target.value));
              setPage(1);
            }}
            className="rounded border border-slate-300 p-1 bg-white font-medium"
          >
            <option value={5}>5</option>
            <option value={10}>10</option>
            <option value={20}>20</option>
            <option value={50}>50</option>
          </select>
        </div>
      </div>

      {/* Table */}
      <div className="overflow-x-auto rounded-lg border border-slate-200 bg-white shadow-xs">
        <table className="w-full text-left text-xs">
          <thead className="border-b border-slate-200 bg-slate-50 font-semibold text-slate-600">
            <tr>
              <th className="p-3">National ID</th>
              <th className="p-3">Full Name</th>
              <th className="p-3">Citizen ID</th>
              <th className="p-3">Governorate</th>
              <th className="p-3">Voting Token</th>
              <th className="p-3 text-right">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-100 text-slate-700">
            {isLoading ? (
              <tr>
                <td colSpan={6} className="p-6 text-center text-slate-500">
                  Loading voters...
                </td>
              </tr>
            ) : pagedData?.data && pagedData.data.length > 0 ? (
              pagedData.data.map((v) => (
                <tr
                  key={v.id}
                  className="hover:bg-slate-50/80 transition-colors"
                >
                  <td className="p-3 font-mono font-medium text-slate-900">
                    {v.nationalId}
                  </td>
                  <td className="p-3 font-medium">
                    {v.firstName} {v.lastName}
                  </td>
                  <td className="p-3 font-mono">#{v.citizenId}</td>
                  <td className="p-3">
                    {GOVERNORATE_NAMES[v.governorate] ?? "Unknown"}
                  </td>
                  <td className="p-3 font-mono text-slate-500 max-w-xs truncate">
                    {v.votingToken}
                  </td>
                  <td className="p-3 text-right">
                    <div className="flex items-center justify-end gap-1.5">
                      <button
                        onClick={() =>
                          navigate("/voters/details", {
                            state: { nationalId: v.nationalId },
                          })
                        }
                        className="rounded px-2 py-1 text-xs text-slate-600 hover:bg-slate-100"
                      >
                        View
                      </button>
                      <button
                        onClick={() =>
                          navigate("/voters/token", {
                            state: { nationalId: v.nationalId },
                          })
                        }
                        className="rounded px-2 py-1 text-xs text-blue-600 hover:bg-blue-50"
                      >
                        Token
                      </button>
                      <button
                        onClick={() =>
                          navigate("/voters/delete", {
                            state: { nationalId: v.nationalId },
                          })
                        }
                        className="rounded px-2 py-1 text-xs text-red-600 hover:bg-red-50"
                      >
                        Delete
                      </button>
                    </div>
                  </td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan={6} className="p-6 text-center text-slate-400">
                  No voter records found.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      {/* Pagination Controls */}
      <div className="flex items-center justify-between text-xs text-slate-600">
        <span>
          Page {pagedData?.currentPage ?? page} of {pagedData?.totalPages ?? 1}
        </span>
        <div className="flex items-center gap-2">
          <button
            onClick={() => setPage((prev) => Math.max(prev - 1, 1))}
            disabled={page === 1 || !pagedData?.hasPreviousPage}
            className="rounded border border-slate-300 px-3 py-1.5 hover:bg-slate-100 disabled:opacity-40"
          >
            Previous
          </button>
          <button
            onClick={() => setPage((prev) => prev + 1)}
            disabled={isPlaceholderData || !pagedData?.hasNextPage}
            className="rounded border border-slate-300 px-3 py-1.5 hover:bg-slate-100 disabled:opacity-40"
          >
            Next
          </button>
        </div>
      </div>
    </div>
  );
};
