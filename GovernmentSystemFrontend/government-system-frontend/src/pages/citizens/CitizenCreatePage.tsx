import { useNavigate } from "react-router-dom";
import { CitizenForm } from "../../components/citizens/CitizenForm";
import { useCitizenMutations } from "../../hooks/useCitizens";
import type { CitizenFormValues } from "../../schemas/citizen.schemas";

export const CitizenCreatePage = () => {
  const navigate = useNavigate();
  const { createCitizen, isCreating } = useCitizenMutations();

  const handleCreate = (formData: CitizenFormValues) => {
    createCitizen(
      {
        ...formData,
        governorate: Number(formData.governorate),
      },
      {
        onSuccess: () => {
          navigate("/citizens");
        },
      },
    );
  };

  return (
    <div className="space-y-6">
      <div className="border-b border-slate-200 pb-4">
        <h1 className="text-xl font-bold text-slate-900">Register Citizen</h1>
        <p className="text-xs text-slate-500 mt-1">
          National ID will be generated automatically upon registration.
        </p>
      </div>

      <CitizenForm
        onSubmit={handleCreate}
        isSubmitting={isCreating}
        submitLabel="Register Citizen"
        onCancel={() => navigate("/citizens")}
      />
    </div>
  );
};
