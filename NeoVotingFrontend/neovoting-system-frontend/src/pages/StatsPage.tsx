import {
  useActiveElection,
  useActivePoll,
  usePagedElections,
  usePagedPolls,
} from "../hooks/useGeneral";

const LoadingText = ({ children }: { children: string }) => (
  <p className="mt-2 text-xs text-slate-400">{children}</p>
);

const ErrorBox = ({
  message,
  onRetry,
}: {
  message: string;
  onRetry: () => void;
}) => (
  <div
    role="alert"
    className="mt-2 flex items-center justify-between gap-3 rounded-md border border-red-200 bg-red-50 px-3 py-2 text-xs text-red-700"
  >
    <span>{message}</span>
    <button
      type="button"
      onClick={onRetry}
      className="rounded border border-red-300 px-2 py-1 font-medium hover:bg-red-100"
    >
      Retry
    </button>
  </div>
);

export const StatsPage = () => {
  const {
    data: activeElection,
    isLoading: loadingActiveElection,
    isError: isElectionError,
    refetch: refetchActiveElection,
  } = useActiveElection();

  const {
    data: activePoll,
    isLoading: loadingActivePoll,
    isError: isPollError,
    refetch: refetchActivePoll,
  } = useActivePoll();

  const {
    data: elections,
    isLoading: loadingElections,
    isError: isElectionsError,
    refetch: refetchElections,
  } = usePagedElections(1, 5);

  const {
    data: polls,
    isLoading: loadingPolls,
    isError: isPollsError,
    refetch: refetchPolls,
  } = usePagedPolls(1, 5);

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold tracking-tight text-slate-900">
          Election System Public Overview
        </h1>
        <p className="text-sm text-slate-500">
          Live active events and public system statistics
        </p>
      </div>

      <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
        {/* Active Election Box */}
        <div className="rounded-xl border border-slate-200 bg-white p-6 shadow-xs">
          <h3 className="text-lg font-semibold text-slate-800">
            Active Election
          </h3>
          {loadingActiveElection ? (
            <LoadingText>Loading active election...</LoadingText>
          ) : isElectionError ? (
            <ErrorBox
              message="Failed to load the active election."
              onRetry={() => refetchActiveElection()}
            />
          ) : activeElection ? (
            <div className="mt-3 space-y-1 text-sm text-slate-600">
              <p>
                <strong>Name:</strong> {activeElection.name}
              </p>
              <p>
                <strong>Status:</strong> {activeElection.status}
              </p>
              <p>
                <strong>Voting Period:</strong> {activeElection.votingStartDate}{" "}
                to {activeElection.votingEndDate}
              </p>
            </div>
          ) : (
            <p className="mt-2 text-xs text-slate-400">
              No active election found.
            </p>
          )}
        </div>

        {/* Active Poll Box */}
        <div className="rounded-xl border border-slate-200 bg-white p-6 shadow-xs">
          <h3 className="text-lg font-semibold text-slate-800">Active Poll</h3>
          {loadingActivePoll ? (
            <LoadingText>Loading active poll...</LoadingText>
          ) : isPollError ? (
            <ErrorBox
              message="Failed to load the active poll."
              onRetry={() => refetchActivePoll()}
            />
          ) : activePoll ? (
            <div className="mt-3 space-y-1 text-sm text-slate-600">
              <p>
                <strong>Name:</strong> {activePoll.name}
              </p>
              <p>
                <strong>Question:</strong> {activePoll.question}
              </p>
              <p>
                <strong>Status:</strong> {activePoll.status}
              </p>
            </div>
          ) : (
            <p className="mt-2 text-xs text-slate-400">No active poll found.</p>
          )}
        </div>
      </div>

      {/* Recent Elections & Polls Lists */}
      <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
        <div className="rounded-xl border border-slate-200 bg-white p-6 shadow-xs">
          <h3 className="mb-3 text-md font-semibold">Recent Elections</h3>
          {loadingElections ? (
            <LoadingText>Loading elections...</LoadingText>
          ) : isElectionsError ? (
            <ErrorBox
              message="Failed to load recent elections."
              onRetry={() => refetchElections()}
            />
          ) : elections?.length ? (
            <ul className="space-y-2 text-xs">
              {elections.map((e) => (
                <li key={e.id} className="rounded bg-slate-50 p-2">
                  <span className="font-semibold">{e.name}</span> ({e.status})
                </li>
              ))}
            </ul>
          ) : (
            <p className="text-xs text-slate-400">No elections yet.</p>
          )}
        </div>

        <div className="rounded-xl border border-slate-200 bg-white p-6 shadow-xs">
          <h3 className="mb-3 text-md font-semibold">Recent Polls</h3>
          {loadingPolls ? (
            <LoadingText>Loading polls...</LoadingText>
          ) : isPollsError ? (
            <ErrorBox
              message="Failed to load recent polls."
              onRetry={() => refetchPolls()}
            />
          ) : polls?.length ? (
            <ul className="space-y-2 text-xs">
              {polls.map((p) => (
                <li key={p.id} className="rounded bg-slate-50 p-2">
                  <span className="font-semibold">{p.name}</span> - {p.question}
                </li>
              ))}
            </ul>
          ) : (
            <p className="text-xs text-slate-400">No polls yet.</p>
          )}
        </div>
      </div>
    </div>
  );
};

export default StatsPage;
