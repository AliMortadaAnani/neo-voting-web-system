// pages/candidates/CandidatePage.tsx
import { FeatureCard } from "../../components/general/FeatureCard";
import { MainPageHeader } from "../../components/general/MainPageHeader";
export const CandidatePage = () => {
  return (
    <div className="w-full py-6">
      <MainPageHeader
        title="Candidates Management"
        subtitle="Republic of Lebanon - Candidate Services"
      />

      <div className="mx-auto flex max-w-xl flex-col gap-3">
        <FeatureCard label="View Candidates List" link="/candidates/list" />
        <FeatureCard
          label="Search & View Candidate Details"
          link="/candidates/details"
        />
        <FeatureCard
          label="Register New Candidate"
          link="/candidates/register"
        />
        <FeatureCard
          label="Generate Candidate Token"
          link="/candidates/new-token"
        />
        <FeatureCard
          label="Delete Candidate Record"
          link="/candidates/delete"
        />
      </div>
    </div>
  );
};
