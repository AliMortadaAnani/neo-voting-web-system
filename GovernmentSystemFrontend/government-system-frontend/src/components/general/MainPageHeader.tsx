type MainPageHeaderProps = {
  title: string;
  subtitle: string;
};

export const MainPageHeader = ({ title, subtitle }: MainPageHeaderProps) => {
  return (
    <div className="mx-auto mb-8 max-w-xl text-center">
      <h1 className="text-2xl font-bold tracking-tight text-red-600 sm:text-3xl">
        {title}
      </h1>
      <p className="mt-1 text-xs font-semibold tracking-wider text-emerald-700 uppercase">
        {subtitle}
      </p>
    </div>
  );
};
