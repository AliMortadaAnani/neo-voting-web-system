// pages/citizens/CitizenDetailsPage.tsx
import { useState } from "react";
import { useLocation } from "react-router-dom";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { fetchCitizenFormSchema } from "../../schemas/citizen.schemas";
import {
  type CitizenFetchRequestDTO,
  GOVERNORATE_NAMES,
} from "../../types/citizen.types";
import { useCitizen } from "../../hooks/useCitizens";
import { Button } from "../../components/general/Button";
import { InputField } from "../../components/general/InputField";
import { LoadingSpinner } from "../../components/general/LoadingSpinner";

export const CitizenSearchPage = () => {
  const location = useLocation();

  // Pick up the ID from the direct React prop FIRST; if none, check router navigation state
  const effectiveNationalId = (location.state as { initialNationalId?: string })
    ?.initialNationalId;

  // Initialize search immediately if an ID was passed via either method
  const [searchDto, setSearchDto] = useState<CitizenFetchRequestDTO | null>(
    effectiveNationalId ? { nationalId: effectiveNationalId } : null,
  );

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<CitizenFetchRequestDTO>({
    resolver: zodResolver(fetchCitizenFormSchema),
    defaultValues: {
      nationalId: effectiveNationalId || "",
    },
  });

  const { citizen, isFetchingCitizen, isFetchCitizenError, fetchCitizenError } =
    useCitizen(searchDto);

  const onSubmit = (data: CitizenFetchRequestDTO) => {
    setSearchDto(data);
  };

  return (
    <div className="mx-auto w-full max-w-2xl py-4">
      {/* ── Search Form Card ── */}
      <div className="rounded-xl border border-stone-200 bg-white p-6 shadow-sm">
        <div className="mb-4">
          <h1 className="text-xl font-bold tracking-tight text-red-600 sm:text-2xl">
            Citizen Record Lookup
          </h1>
          <p className="mt-1 text-xs font-semibold tracking-wider text-emerald-700 uppercase">
            Civil Registry Inquiries
          </p>
        </div>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <InputField
            label="National ID Number"
            type="text"
            placeholder="e.g. 1029384756"
            disabled={isFetchingCitizen}
            error={errors.nationalId?.message}
            {...register("nationalId")}
          />

          <div className="flex justify-end">
            <Button
              label={isFetchingCitizen ? "Searching..." : "Search Record"}
              color="green"
              size="md"
              isLoading={isFetchingCitizen}
            />
          </div>
        </form>
      </div>

      {/* ── Results Area ── */}
      <div className="mt-6">
        {isFetchingCitizen && (
          <div className="rounded-xl border border-stone-200 bg-white p-8">
            <LoadingSpinner
              fullScreen={false}
              label="Searching Civil Registry records..."
            />
          </div>
        )}

        {isFetchCitizenError && !isFetchingCitizen && (
          <div className="rounded-xl border border-red-200 bg-red-50 p-4 text-center">
            <h3 className="text-sm font-bold text-red-700">Record Not Found</h3>
            <p className="mt-1 text-xs text-red-600">
              {fetchCitizenError?.message ||
                "No registered citizen matches this National ID."}
            </p>
          </div>
        )}

        {citizen && !isFetchingCitizen && !isFetchCitizenError && (
          <div className="rounded-xl border border-stone-200 bg-white p-6 shadow-sm">
            <div className="flex flex-col justify-between gap-2 border-b border-stone-100 pb-4 sm:flex-row sm:items-center">
              <div>
                <h2 className="text-lg font-bold text-stone-900">
                  {citizen.firstName} {citizen.lastName}
                </h2>
                <span className="text-xs text-stone-500">
                  System Registry ID #{citizen.id}
                </span>
              </div>
              <div className="self-start sm:self-auto">
                <span className="inline-block rounded-md border border-emerald-200 bg-emerald-50 px-2.5 py-1 text-xs font-bold text-emerald-800">
                  ID: {citizen.nationalId}
                </span>
              </div>
            </div>

            <div className="mt-4 grid grid-cols-1 gap-4 sm:grid-cols-3">
              <div className="rounded-lg border border-stone-100 bg-stone-50 p-3">
                <span className="block text-[11px] font-semibold tracking-wider text-stone-500 uppercase">
                  Date of Birth
                </span>
                <p className="mt-1 text-sm font-semibold text-stone-800">
                  {citizen.dateOfBirth}
                </p>
              </div>

              <div className="rounded-lg border border-stone-100 bg-stone-50 p-3">
                <span className="block text-[11px] font-semibold tracking-wider text-stone-500 uppercase">
                  Gender
                </span>
                <p className="mt-1 text-sm font-semibold text-stone-800">
                  {citizen.gender.toUpperCase() === "M" ? "Male" : "Female"}
                </p>
              </div>

              <div className="rounded-lg border border-stone-100 bg-stone-50 p-3">
                <span className="block text-[11px] font-semibold tracking-wider text-stone-500 uppercase">
                  Governorate
                </span>
                <p className="mt-1 text-sm font-semibold text-stone-800">
                  {GOVERNORATE_NAMES[citizen.governorate] ||
                    citizen.governorate}
                </p>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};
