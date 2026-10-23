// pages/voters/VoterPage.tsx
import { FeatureCard } from "../../components/general/FeatureCard";
import { MainPageHeader } from "../../components/general/MainPageHeader";
export const VoterPage = () => {
  return (
    <div className="w-full py-6">
      <MainPageHeader
        title="Voters Management"
        subtitle="Republic of Lebanon - Voter Services"
      />
      <div className="mx-auto flex max-w-xl flex-col gap-3">
        <FeatureCard label="View Voters List" link="/voters/list" />
        <FeatureCard
          label="Search & View Voter Details"
          link="/voters/details"
        />
        <FeatureCard label="Register New Voter" link="/voters/register" />
        <FeatureCard label="Generate Voter Token" link="/voters/new-token" />
        <FeatureCard label="Delete Voter Record" link="/voters/delete" />
      </div>
    </div>
  );
};
