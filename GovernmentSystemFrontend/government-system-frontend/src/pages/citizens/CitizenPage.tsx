// Inside pages/CitizenPage.tsx
import { useNavigate } from "react-router-dom";
import { Button } from "../../components/general/Button";
import { FeatureCard } from "../../components/general/FeatureCard";
import { MainPageHeader } from "../../components/general/MainPageHeader";
export const CitizenPage = () => {
  const navigate = useNavigate();

  // Demo 1 Handler: Navigates to /citizens/details passing ID secretly in router state
  const handleRouteDemo = () => {
    navigate("/citizens/details", {
      state: { initialNationalId: "NI-AA2M1403-8X7Ms" },
    });
  };

  return (
    <div className="w-full py-6">
      <MainPageHeader
        title="Civil Registry"
        subtitle="Republic of Lebanon — Citizen Records"
      />

      {/* ── Testing Sandbox: The Two Demo Buttons ── */}
      <div className="mx-auto mb-8 flex max-w-xl flex-wrap items-center justify-center gap-3 rounded-xl border border-dashed border-stone-300 bg-stone-50 p-4">
        {/* Demo 1: Navigate to Route with Hidden State */}
        <Button
          label="Demo 1: Route Navigate (Hidden State)"
          onClick={handleRouteDemo}
          color="stone"
          size="sm"
        />
      </div>

      {/* If Demo 2 is active, render the page component inline right here with the prop */}

      {/* Standard Feature Navigation Cards */}
      <div className="mx-auto flex max-w-xl flex-col gap-3">
        <FeatureCard label="View Citizens Registry" link="/citizens/list" />
        <FeatureCard
          label="Search & View Citizen Details"
          link="/citizens/details"
        />
        <FeatureCard label="Register New Citizen" link="/citizens/register" />
        <FeatureCard label="Update Citizen Details" link="/citizens/edit" />
        <FeatureCard label="Delete Citizen Record" link="/citizens/delete" />
      </div>
    </div>
  );
};
