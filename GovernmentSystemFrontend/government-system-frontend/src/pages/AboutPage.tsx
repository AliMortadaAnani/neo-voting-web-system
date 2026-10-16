// pages/AboutPage.tsx
import { useNavigate, useLocation } from "react-router-dom";
import { Button } from "../components/Button";

export const AboutPage = () => {
  const navigate = useNavigate();
  const location = useLocation();

  // Return to previous route from location state, or fallback to history back (-1)
  const handleBack = () => {
    if (location.state?.from) {
      navigate(location.state.from);
    } else {
      navigate(-1);
    }
  };

  return (
    <div className="mx-auto w-full max-w-3xl py-4">
      {/* Sticky Return Button at Top Left */}
      <div className="top-20 z-10 mb-4 flex justify-start">
        <Button label="← Return" onClick={handleBack} color="stone" size="sm" />
      </div>

      {/* Main Content Card */}
      <div className="rounded-xl border border-stone-200 bg-white p-6 shadow-sm sm:p-8">
        {/* Red Title Accent */}
        <h1 className="text-2xl font-bold text-red-600">About GovPortal</h1>

        {/* Cedar Green Subtitle Badge */}
        <p className="mt-1 text-xs font-semibold tracking-wider text-emerald-700 uppercase">
          Republic of Lebanon — Civil & Electoral Registry
        </p>

        {/* Overview Paragraphs */}
        <div className="mt-6 space-y-4 text-sm leading-relaxed text-gray-700">
          <p>
            GovPortal is an integrated administrative platform engineered to
            modernize civil identity records and electoral operations for the
            Republic of Lebanon.
          </p>
          <p>
            The system manages foundational civil registration across all five
            governorates, enables eligible citizens to enroll as voters, issues
            tamper-evident cryptographic voting and nomination tokens, and
            supervises parliamentary candidate nominations under a secure,
            auditable digital architecture.
          </p>
        </div>
      </div>
    </div>
  );
};
