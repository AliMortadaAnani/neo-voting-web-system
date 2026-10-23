// pages/general/AboutPage.tsx
import { Button } from "../../components/general/Button";
import { useGoBack } from "../../hooks/useUtils";

export const AboutPage = () => {
  const handleBack = useGoBack();

  return (
    <div className="mx-auto flex w-full max-w-3xl flex-col items-start gap-6 py-4 md:flex-row">
      {/* Sticky Return Button at Top Left */}
      <div className="sticky top-5 z-10 mb-4 flex shrink-0 justify-start">
        <Button label="← Return" onClick={handleBack} color="stone" size="sm" />
      </div>

      {/* Main Content Card */}
      <div className="flex-1 rounded-xl border border-stone-200 bg-white p-6 shadow-sm sm:p-8">
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
          <p>
            The system manages foundational civil registration across all five
            governorates, enables eligible citizens to enroll as voters, issues
            tamper-evident cryptographic voting and nomination tokens, and
            supervises parliamentary candidate nominations under a secure,
            auditable digital architecture.
          </p>
          <p>
            The system manages foundational civil registration across all five
            governorates, enables eligible citizens to enroll as voters, issues
            tamper-evident cryptographic voting and nomination tokens, and
            supervises parliamentary candidate nominations under a secure,
            auditable digital architecture.
          </p>
          <p>
            The system manages foundational civil registration across all five
            governorates, enables eligible citizens to enroll as voters, issues
            tamper-evident cryptographic voting and nomination tokens, and
            supervises parliamentary candidate nominations under a secure,
            auditable digital architecture.
          </p>
          <p>
            The system manages foundational civil registration across all five
            governorates, enables eligible citizens to enroll as voters, issues
            tamper-evident cryptographic voting and nomination tokens, and
            supervises parliamentary candidate nominations under a secure,
            auditable digital architecture.
          </p>
          <p>
            The system manages foundational civil registration across all five
            governorates, enables eligible citizens to enroll as voters, issues
            tamper-evident cryptographic voting and nomination tokens, and
            supervises parliamentary candidate nominations under a secure,
            auditable digital architecture.
          </p>
          <p>
            The system manages foundational civil registration across all five
            governorates, enables eligible citizens to enroll as voters, issues
            tamper-evident cryptographic voting and nomination tokens, and
            supervises parliamentary candidate nominations under a secure,
            auditable digital architecture.
          </p>
          <p>
            The system manages foundational civil registration across all five
            governorates, enables eligible citizens to enroll as voters, issues
            tamper-evident cryptographic voting and nomination tokens, and
            supervises parliamentary candidate nominations under a secure,
            auditable digital architecture.
          </p>
          <p>
            The system manages foundational civil registration across all five
            governorates, enables eligible citizens to enroll as voters, issues
            tamper-evident cryptographic voting and nomination tokens, and
            supervises parliamentary candidate nominations under a secure,
            auditable digital architecture.
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
