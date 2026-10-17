// pages/VoterPage.tsx
import { FeatureCard } from "../components/FeatureCard";

export const VoterPage = () => {
  return (
    <div className="w-full py-6">
      {/* Page Header */}
      <div className="mx-auto mb-8 max-w-xl text-center">
        <h1 className="text-2xl font-bold tracking-tight text-red-600 sm:text-3xl">
          Voters Management
        </h1>
        <p className="mt-1 text-xs font-semibold tracking-wider text-emerald-700 uppercase">
          Republic of Lebanon - Voter Services
        </p>
      </div>

      {/* 
        LAYOUT 1: Single-Column Stacked List
        - max-w-xl keeps cards from stretching too wide on wide monitors
        - flex flex-col gap-3 stacks them vertically with even spacing
      */}
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
