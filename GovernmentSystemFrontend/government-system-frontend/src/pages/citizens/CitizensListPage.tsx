import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useCitizens, useCitizensTotalCount } from "../../hooks/useCitizens";
import { GOVERNORATE_NAMES } from "../../types/citizen.types";
export const CitizensListPage = () => {
  const navigate = useNavigate();
  const [page, setPage] = useState<number>(1);
  const [pageSize, setPageSize] = useState<number>(10);

  const {
    data: pagedData,
    isLoading,
    isPlaceholderData,
  } = useCitizens(page, pageSize);
  const { data: totalCount, isLoading: isCountLoading } =
    useCitizensTotalCount();

  return (
    <div className="space-y-6">
      {/* Top Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 border-b border-slate-200 pb-4">
        <div>
          <h1 className="text-xl font-bold text-slate-900">
            Citizens Directory
          </h1>
          <p className="text-xs text-slate-500 mt-1">
            Total Registered:{" "}
            <span className="font-semibold text-slate-800">
              {isCountLoading ? "..." : (totalCount ?? "0")}
            </span>
          </p>
        </div>

        <div className="flex items-center gap-2">
          <Link
            to="/citizens/create"
            className="rounded-lg bg-slate-900 px-4 py-2 text-xs font-semibold text-white hover:bg-slate-800"
          >
            Register Citizen
          </Link>
          <Link
            to="/citizens/details"
            className="rounded-lg bg-slate-900 px-4 py-2 text-xs font-semibold text-white hover:bg-slate-800"
          >
            Search Citizen
          </Link>
        </div>
      </div>

      {/* Filter / Page Size Toolbar */}
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
              <th className="p-3">Date of Birth</th>
              <th className="p-3">Governorate</th>
              <th className="p-3">Gender</th>
              <th className="p-3 text-right">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-100 text-slate-700">
            {isLoading ? (
              <tr>
                <td colSpan={6} className="p-6 text-center text-slate-500">
                  Loading citizens...
                </td>
              </tr>
            ) : pagedData?.data && pagedData.data.length > 0 ? (
              pagedData.data.map((c) => (
                <tr
                  key={c.id}
                  className="hover:bg-slate-50/80 transition-colors"
                >
                  <td className="p-3 font-mono font-medium text-slate-900">
                    {c.nationalId}
                  </td>
                  <td className="p-3 font-medium">
                    {c.firstName} {c.lastName}
                  </td>
                  <td className="p-3">{c.dateOfBirth}</td>
                  <td className="p-3">
                    {GOVERNORATE_NAMES[c.governorate] ?? "Unknown"}
                  </td>
                  <td className="p-3">{c.gender}</td>
                  <td className="p-3 text-right">
                    <div className="flex items-center justify-end gap-1.5">
                      <button
                        onClick={() =>
                          navigate("/citizens/details", {
                            state: { nationalId: c.nationalId },
                          })
                        }
                        className="rounded px-2 py-1 text-xs text-slate-600 hover:bg-slate-100"
                      >
                        View
                      </button>
                      <button
                        onClick={() =>
                          navigate("/citizens/details", {
                            state: { nationalId: c.nationalId },
                          })
                        }
                        className="rounded px-2 py-1 text-xs text-blue-600 hover:bg-blue-50"
                      >
                        Edit
                      </button>
                      <button
                        onClick={() =>
                          navigate("/citizens/delete", {
                            state: { nationalId: c.nationalId },
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
                  No citizen records found.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      {/* Pagination Bar */}
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
