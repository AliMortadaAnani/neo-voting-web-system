import React from "react";
import { Button } from "../../components/general/Button";
import { useCitizens } from "../../hooks/useCitizens";
import type { CitizenResponseDTO } from "../../types/citizen.types";
import { LoadingSpinner } from "../../components/general/LoadingSpinner";
import { ServerErrorState } from "../../components/general/ServerErrorState";
import { Link } from "react-router-dom";

export type Column<T> = {
  header: string;
  key: keyof T;
};

export type TableAction = {
  label: string;
  to: string;
};

type SimpleTableProps<T extends { id: string | number }> = {
  columns: Column<T>[];
  data: T[] | undefined;
  actions?: TableAction[];
  stateProperty?: keyof T; // Optional: specify which property to send as the main ID (defaults to 'id')
};

export const SimpleTable = <T extends { id: string | number }>({
  columns,
  data,
  actions = [],
  stateProperty = "id",
}: SimpleTableProps<T>) => {
  // ── ONE STATE PER TABLE ──
  // Holds the 'id' of the currently open row, or null if all are closed
  const [openRowId, setOpenRowId] = useState<string | number | null>(null);

  const toggleMenu = (id: string | number) => {
    setOpenRowId((current) => (current === id ? null : id));
  };

  const closeMenu = () => setOpenRowId(null);

  const hasActions = actions.length > 0;
  const totalColSpan = columns.length + (hasActions ? 1 : 0);

  return (
    /* 1. Notice: 'overflow-hidden' was REMOVED from this outer wrapper */
    <div className="w-full rounded-xl border border-stone-200 bg-white shadow-sm">
      {/* Invisible backdrop to dismiss menu on outside click */}
      {openRowId !== null && (
        <div
          className="fixed inset-0 z-20 cursor-default"
          onClick={closeMenu}
        />
      )}

      {/* Minimum height prevents cramped tables from squeezing menus */}
      <div className="min-h-[160px] overflow-x-auto pb-4">
        <table className="w-full text-left text-sm">
          {/* Header */}
          <thead className="border-b border-stone-200 bg-stone-50 text-[11px] font-semibold tracking-wider text-stone-500 uppercase">
            <tr>
              {columns.map((col) => (
                <th key={String(col.key)} className="px-5 py-3.5">
                  {col.header}
                </th>
              ))}
              {hasActions && (
                <th className="px-5 py-3.5 text-right">Actions</th>
              )}
            </tr>
          </thead>

          {/* Body */}
          <tbody className="divide-y divide-stone-100">
            {data === undefined ? (
              <tr>
                <td
                  colSpan={totalColSpan}
                  className="px-5 py-8 text-center text-stone-500"
                >
                  Loading...
                </td>
              </tr>
            ) : data.length === 0 ? (
              <tr>
                <td
                  colSpan={totalColSpan}
                  className="px-5 py-8 text-center text-stone-500"
                >
                  No records found.
                </td>
              </tr>
            ) : (
              data.map((item, index) => {
                const isOpen = openRowId === item.id;

                // 2. Flip upward if this row is near the bottom of the list
                const isNearBottom =
                  data.length > 2 && index >= data.length - 2;

                const stateValue = item[stateProperty];
                const routerState = {
                  id: item.id,
                  initialNationalId:
                    (item as Record<string, unknown>).nationalId ?? stateValue,
                  [stateProperty]: stateValue,
                };

                return (
                  <tr
                    key={item.id}
                    className="transition-colors hover:bg-stone-50/75"
                  >
                    {columns.map((col) => (
                      <td
                        key={String(col.key)}
                        className="px-5 py-3.5 text-stone-800"
                      >
                        {item[col.key] as React.ReactNode}
                      </td>
                    ))}

                    {/* 3. Actions cell: elevates to z-30 when open */}
                    {hasActions && (
                      <td
                        className={`relative px-5 py-3.5 text-right ${
                          isOpen ? "z-30" : "z-0"
                        }`}
                      >
                        <button
                          type="button"
                          onClick={() => toggleMenu(item.id)}
                          className="inline-flex h-8 w-8 items-center justify-center rounded-lg text-stone-500 transition-colors hover:bg-stone-100 hover:text-stone-800 focus:outline-none"
                        >
                          <span className="text-base leading-none font-bold">
                            ⋯
                          </span>
                        </button>

                        {/* Dropdown Menu */}
                        {isOpen && (
                          <div
                            className={`animate-in fade-in zoom-in-95 absolute right-4 z-40 min-w-36 rounded-lg border border-stone-200 bg-white py-1 shadow-xl duration-100 ${
                              isNearBottom
                                ? "bottom-full mb-1.5" // Opens upward
                                : "top-full mt-1.5" // Opens downward
                            }`}
                          >
                            {actions.map((action) => (
                              <Link
                                key={action.label}
                                to={action.to}
                                state={routerState}
                                onClick={closeMenu}
                                className="block px-4 py-2 text-left text-xs font-semibold text-stone-700 transition-colors hover:bg-emerald-50 hover:text-emerald-700"
                              >
                                {action.label}
                              </Link>
                            ))}
                          </div>
                        )}
                      </td>
                    )}
                  </tr>
                );
              })
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
};

import { useState } from "react";

import {
  SelectField,
  type SelectOption,
} from "../../components/general/SelectField";

import type { PagedResult } from "../../types/citizen.types";
import type { AppError } from "../../api/api";

const defaultPageSizeOptions: SelectOption[] = [
  { value: 5, label: "5 per page" },
  { value: 10, label: "10 per page" },
  { value: 15, label: "15 per page" },
  { value: 20, label: "20 per page" },
];
const citizenActions: TableAction[] = [
  { label: "View Details", to: "/citizens/details" },
  { label: "Edit Citizen", to: "/citizens/edit" },
  { label: "Delete Record", to: "/citizens/delete" },
];

type PagedTableProps<T extends { id: string | number }> = {
  title?: string;
  columns: Column<T>[];
  pagedData?: PagedResult<T>;
  isLoading: boolean;
  isError: boolean;
  error?: AppError | null;
  onPageChange: (newPage: number) => void;
  onPageSizeChange: (newPageSize: number) => void;
};

export const PagedTable = <T extends { id: string | number }>({
  title,
  columns,
  pagedData,
  isLoading,
  isError,
  error,
  onPageChange,
  onPageSizeChange,
}: PagedTableProps<T>) => {
  // 1. Initial full-screen or container loading state
  if (isLoading && !pagedData) {
    return (
      <div className="flex min-h-[300px] items-center justify-center rounded-xl border border-stone-200 bg-white p-8">
        <LoadingSpinner
          fullScreen={false}
          label="Loading registry records..."
        />
      </div>
    );
  }

  // 2. Error state
  if (isError) {
    return <ServerErrorState error={error} fullScreen={false} />;
  }

  const currentPage = pagedData?.currentPage ?? 1;
  const totalPages = pagedData?.totalPages ?? 1;
  const totalCount = pagedData?.totalCount ?? 0;
  const pageSize = pagedData?.pageSize ?? 10;
  const hasPrevious = pagedData?.hasPreviousPage ?? false;
  const hasNext = pagedData?.hasNextPage ?? false;

  const handlePageSizeChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    onPageSizeChange(Number(e.target.value));
    onPageChange(1); // Reset to page 1 safely when size changes
  };

  return (
    <div className="w-full space-y-4">
      {/* Top Header & Page Size Select */}
      <div className="flex flex-col gap-3 sm:flex-row sm:items-end sm:justify-between">
        <div>
          {title && (
            <h2 className="text-xl font-bold tracking-tight text-red-600 sm:text-2xl">
              {title}
            </h2>
          )}
          <span className="text-xs font-semibold tracking-wider text-emerald-700 uppercase">
            Page {currentPage} of {totalPages}
          </span>
        </div>

        <div className="w-36">
          <SelectField
            label="Page Size"
            options={defaultPageSizeOptions}
            value={pageSize}
            onChange={handlePageSizeChange}
          />
        </div>
      </div>

      {/* The Generic Table */}
      <SimpleTable
        columns={columns}
        data={pagedData?.data ?? []}
        actions={citizenActions}
      />

      {/* Pagination Controls */}
      <div className="flex items-center justify-between pt-2">
        <Button
          label="← Previous"
          color="stone"
          size="sm"
          onClick={() => hasPrevious && onPageChange(currentPage - 1)}
        />

        <span className="text-xs text-stone-500">
          Showing Page {currentPage} of {totalPages} ({totalCount} total
          records)
        </span>

        <Button
          label="Next →"
          color="stone"
          size="sm"
          onClick={() => hasNext && onPageChange(currentPage + 1)}
        />
      </div>
    </div>
  );
};

export const Test = () => {
  const [page, setPage] = useState<number>(1);
  const [pageSize, setPageSize] = useState<number>(10);

  const {
    citizens,
    isFetchingCitizens,
    isFetchCitizensError,
    fetchCitizensError,
  } = useCitizens(page, pageSize);

  const citizenColumns: Column<CitizenResponseDTO>[] = [
    { header: "Citizen ID", key: "nationalId" },
    { header: "First Name", key: "firstName" },
    { header: "Last Name", key: "lastName" },
    { header: "Date of Birth", key: "dateOfBirth" },
    { header: "Gender", key: "gender" },
  ];

  return (
    <div className="mx-auto max-w-5xl py-6">
      <PagedTable
        title="Citizens Registry"
        columns={citizenColumns}
        pagedData={citizens}
        isLoading={isFetchingCitizens}
        isError={isFetchCitizensError}
        error={fetchCitizensError}
        onPageChange={setPage}
        onPageSizeChange={setPageSize}
      />
    </div>
  );
};
