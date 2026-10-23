// components/FeatureCard.tsx
import { Link } from "react-router-dom";

type FeatureCardProps = {
  label: string;
  link: string;
};

export const FeatureCard = ({ label, link }: FeatureCardProps) => {
  return (
    <Link
      key={link}
      to={link}
      /*
        'group' allows child elements (like the text and arrow) to react when the parent card is hovered.
        'block w-full' ensures it adapts naturally to flex columns or grid slots set by the parent.
      */
      className="group block w-full rounded-xl border border-stone-200 bg-white px-5 py-4 shadow-sm transition-all duration-150 ease-in-out hover:border-emerald-600 hover:bg-emerald-50/40 hover:shadow-md active:scale-[0.99] active:bg-emerald-100/60"
    >
      <div className="flex items-center justify-between gap-4">
        {/* Label: turns cedar green when the card is hovered */}
        <span className="text-sm font-semibold text-stone-800 transition-colors group-hover:text-emerald-700 sm:text-base">
          {label}
        </span>

        {/* Right Arrow Indicator: slides right and turns green on hover */}
        <span className="text-lg font-bold text-stone-400 transition-all group-hover:translate-x-1 group-hover:text-emerald-600 sm:text-xl">
          →
        </span>
      </div>
    </Link>
  );
};
