import { useQuery } from "@tanstack/react-query";
import { testAdmin } from "../services/auth.services";
import type { AppError } from "../api/api";

export const AdminPage = () => {
  const { data, isLoading, isError, error, refetch } = useQuery<
    string,
    AppError
  >({
    queryKey: ["test-admin"],
    queryFn: testAdmin,
  });

  return (
    <div className="rounded-xl border border-slate-200 bg-white p-6 shadow-xs">
      <h1 className="text-2xl font-bold tracking-tight text-slate-900">
        Admin Dashboard
      </h1>
      <p className="mt-1 text-sm text-slate-500">
        Role-authorized panel for system administrators.
      </p>

      {/* Live Server Authorization Status */}
      <div className="mt-6 rounded-lg bg-slate-50 p-4 border border-slate-200">
        <span className="text-xs font-semibold uppercase text-slate-400">
          Server Response (GET /Auth/test-admin)
        </span>

        {isLoading && (
          <p className="mt-2 text-sm text-slate-600">
            Verifying authorization with backend...
          </p>
        )}

        {isError && (
          <p className="mt-2 text-sm font-medium text-red-600">
            Authorization Failed: {error.message}
          </p>
        )}

        {data && (
          <p className="mt-2 text-sm font-medium text-emerald-700">✓ {data}</p>
        )}

        <button
          onClick={() => refetch()}
          className="mt-4 rounded-md bg-slate-900 px-3 py-1.5 text-xs font-semibold text-white hover:bg-slate-800"
        >
          Retest Endpoint
        </button>
      </div>
    </div>
  );
};

export default AdminPage;
