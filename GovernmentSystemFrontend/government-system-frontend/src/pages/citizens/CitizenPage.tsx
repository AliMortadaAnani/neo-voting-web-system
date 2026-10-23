// pages/citizens/CitizenPage.tsx
import { FeatureCard } from "../../components/general/FeatureCard";
import { MainPageHeader } from "../../components/general/MainPageHeader";

export const CitizenPage = () => {
  return (
    <div className="w-full py-6">
      <MainPageHeader
        title="Citizens Management"
        subtitle="Republic of Lebanon - Citizen Services"
      />

      <div className="mx-auto flex max-w-xl flex-col gap-3">
        <FeatureCard label="View Citizens List" link="/citizens/list" />
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
