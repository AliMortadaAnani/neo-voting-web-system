import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  citizenFormSchema,
  type CitizenFormValues,
} from "../../schemas/citizen.schemas";
import { Governorate, GOVERNORATE_NAMES } from "../../types/citizen.types";

interface CitizenFormProps {
  initialValues?: Partial<CitizenFormValues>;
  nationalId?: string;
  onSubmit: (data: CitizenFormValues) => void;
  isSubmitting: boolean;
  submitLabel?: string;
  onCancel?: () => void;
}

export const CitizenForm = ({
  initialValues,
  nationalId,
  onSubmit,
  isSubmitting,
  submitLabel = "Save Citizen",
  onCancel,
}: CitizenFormProps) => {
  // Plain RHF initialization:
  // Because CitizenEditPage has key={citizen.nationalId}, this runs fresh on mount
  // and will NEVER overwrite user typing during submit!
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<CitizenFormValues>({
    resolver: zodResolver(citizenFormSchema),
    defaultValues: {
      firstName: initialValues?.firstName ?? "",
      lastName: initialValues?.lastName ?? "",
      dateOfBirth: initialValues?.dateOfBirth?.split("T")[0] ?? "",
      governorate: initialValues?.governorate
        ? String(initialValues.governorate)
        : "1",
      gender: initialValues?.gender ?? "M",
    },
  });

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4 max-w-xl">
      {/* Read-Only National ID in Edit Mode */}
      {nationalId && (
        <div>
          <label className="block text-xs font-semibold uppercase text-slate-500 mb-1">
            National ID (Immutable)
          </label>
          <input
            type="text"
            value={nationalId}
            disabled
            className="w-full rounded-lg border border-slate-200 bg-slate-100 p-2.5 text-sm text-slate-500 font-mono cursor-not-allowed"
          />
        </div>
      )}

      {/* First & Last Name */}
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div>
          <label className="block text-xs font-semibold uppercase text-slate-700 mb-1">
            First Name
          </label>
          <input
            type="text"
            placeholder="John"
            {...register("firstName")}
            className="w-full rounded-lg border border-slate-300 p-2.5 text-sm focus:border-slate-900 focus:outline-none"
          />
          {errors.firstName && (
            <p className="mt-1 text-xs text-red-600">
              {errors.firstName.message}
            </p>
          )}
        </div>

        <div>
          <label className="block text-xs font-semibold uppercase text-slate-700 mb-1">
            Last Name
          </label>
          <input
            type="text"
            placeholder="Doe"
            {...register("lastName")}
            className="w-full rounded-lg border border-slate-300 p-2.5 text-sm focus:border-slate-900 focus:outline-none"
          />
          {errors.lastName && (
            <p className="mt-1 text-xs text-red-600">
              {errors.lastName.message}
            </p>
          )}
        </div>
      </div>

      {/* Date of Birth & Gender */}
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div>
          <label className="block text-xs font-semibold uppercase text-slate-700 mb-1">
            Date of Birth (18+)
          </label>
          <input
            type="date"
            {...register("dateOfBirth")}
            className="w-full rounded-lg border border-slate-300 p-2.5 text-sm focus:border-slate-900 focus:outline-none"
          />
          {errors.dateOfBirth && (
            <p className="mt-1 text-xs text-red-600">
              {errors.dateOfBirth.message}
            </p>
          )}
        </div>

        <div>
          <label className="block text-xs font-semibold uppercase text-slate-700 mb-1">
            Gender
          </label>
          <select
            {...register("gender")}
            className="w-full rounded-lg border border-slate-300 p-2.5 text-sm focus:border-slate-900 focus:outline-none bg-white"
          >
            <option value="M">Male (M)</option>
            <option value="F">Female (F)</option>
          </select>
          {errors.gender && (
            <p className="mt-1 text-xs text-red-600">{errors.gender.message}</p>
          )}
        </div>
      </div>

      {/* Governorate */}
      <div>
        <label className="block text-xs font-semibold uppercase text-slate-700 mb-1">
          Governorate
        </label>
        <select
          {...register("governorate")}
          className="w-full rounded-lg border border-slate-300 p-2.5 text-sm focus:border-slate-900 focus:outline-none bg-white"
        >
          <option value={Governorate.Beirut}>Beirut (1)</option>
          <option value={Governorate.MountLebanon}>Mount Lebanon (2)</option>
          <option value={Governorate.South}>South (3)</option>
          <option value={Governorate.East}>East (4)</option>
          <option value={Governorate.North}>North (5)</option>
        </select>
        {errors.governorate && (
          <p className="mt-1 text-xs text-red-600">
            {errors.governorate.message}
          </p>
        )}
      </div>

      {/* Buttons */}
      <div className="flex items-center gap-3 pt-2">
        <button
          type="submit"
          disabled={isSubmitting}
          className="rounded-lg bg-slate-900 px-4 py-2.5 text-sm font-semibold text-white hover:bg-slate-800 disabled:opacity-50"
        >
          {isSubmitting ? "Saving..." : submitLabel}
        </button>
        {onCancel && (
          <button
            type="button"
            onClick={onCancel}
            className="rounded-lg border border-slate-300 px-4 py-2.5 text-sm font-medium text-slate-700 hover:bg-slate-50"
          >
            Cancel
          </button>
        )}
      </div>
    </form>
  );
};
