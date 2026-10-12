import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  electionCreateSchema,
  pollCreateSchema,
} from "../schemas/admin.schemas";
import type {
  ElectionCreateRequestDTO,
  PollCreateRequestDTO,
} from "../types/admin.types";
import { useAdminMutations, useSystemAuditLogs } from "../hooks/useAdmin";
import { usePagedElections, usePagedPolls } from "../hooks/useGeneral";

export const AdminPage = () => {
  const [auditActionType, setAuditActionType] = useState<number | undefined>();
  const [auditAdminId, setAuditAdminId] = useState<number | undefined>();

  const { data: auditLogs } = useSystemAuditLogs(
    auditActionType,
    auditAdminId,
    1,
    10,
  );
  const { data: elections } = usePagedElections(1, 5);
  const { data: polls } = usePagedPolls(1, 5);

  const {
    createElection,
    createPoll,
    startElection,
    completeElection,
    startPoll,
    completePoll,
    isCreatingElection,
    isCreatingPoll,
  } = useAdminMutations();

  const electionForm = useForm<ElectionCreateRequestDTO>({
    resolver: zodResolver(electionCreateSchema),
    defaultValues: {
      name: "",
      nominationStartDate: "",
      nominationEndDate: "",
      votingStartDate: "",
      votingEndDate: "",
    },
  });

  const pollForm = useForm<PollCreateRequestDTO>({
    resolver: zodResolver(pollCreateSchema),
    defaultValues: {
      name: "",
      question: "",
      startDate: "",
      endDate: "",
      answers: ["", ""],
    },
  });

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-2xl font-bold tracking-tight text-slate-900">
          Admin Command Center
        </h1>
        <p className="text-sm text-slate-500">
          Manage elections, polls, and review system audit records.
        </p>
      </div>

      <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
        {/* Create Election Form */}
        <div className="rounded-xl border border-slate-200 bg-white p-6 shadow-xs">
          <h3 className="mb-4 text-lg font-semibold">Create Election</h3>
          <form
            onSubmit={electionForm.handleSubmit((d) => createElection(d))}
            className="space-y-3"
          >
            <div>
              <label className="text-xs font-medium">Name</label>
              <input
                {...electionForm.register("name")}
                className="w-full rounded border px-3 py-1.5 text-sm"
              />
            </div>
            <div className="grid grid-cols-2 gap-2">
              <div>
                <label className="text-xs font-medium">Nomination Start</label>
                <input
                  type="datetime-local"
                  {...electionForm.register("nominationStartDate")}
                  className="w-full rounded border px-2 py-1.5 text-xs"
                />
              </div>
              <div>
                <label className="text-xs font-medium">Nomination End</label>
                <input
                  type="datetime-local"
                  {...electionForm.register("nominationEndDate")}
                  className="w-full rounded border px-2 py-1.5 text-xs"
                />
              </div>
            </div>
            <div className="grid grid-cols-2 gap-2">
              <div>
                <label className="text-xs font-medium">Voting Start</label>
                <input
                  type="datetime-local"
                  {...electionForm.register("votingStartDate")}
                  className="w-full rounded border px-2 py-1.5 text-xs"
                />
              </div>
              <div>
                <label className="text-xs font-medium">Voting End</label>
                <input
                  type="datetime-local"
                  {...electionForm.register("votingEndDate")}
                  className="w-full rounded border px-2 py-1.5 text-xs"
                />
              </div>
            </div>
            <button
              type="submit"
              disabled={isCreatingElection}
              className="w-full rounded bg-slate-900 py-2 text-sm text-white"
            >
              {isCreatingElection ? "Creating..." : "Create Election"}
            </button>
          </form>
        </div>

        {/* Create Poll Form */}
        <div className="rounded-xl border border-slate-200 bg-white p-6 shadow-xs">
          <h3 className="mb-4 text-lg font-semibold">Create Poll</h3>
          <form
            onSubmit={pollForm.handleSubmit((d) => createPoll(d))}
            className="space-y-3"
          >
            <div>
              <label className="text-xs font-medium">Name</label>
              <input
                {...pollForm.register("name")}
                className="w-full rounded border px-3 py-1.5 text-sm"
              />
            </div>
            <div>
              <label className="text-xs font-medium">Question</label>
              <input
                {...pollForm.register("question")}
                className="w-full rounded border px-3 py-1.5 text-sm"
              />
            </div>
            <div className="grid grid-cols-2 gap-2">
              <div>
                <label className="text-xs font-medium">Start Date</label>
                <input
                  type="datetime-local"
                  {...pollForm.register("startDate")}
                  className="w-full rounded border px-2 py-1.5 text-xs"
                />
              </div>
              <div>
                <label className="text-xs font-medium">End Date</label>
                <input
                  type="datetime-local"
                  {...pollForm.register("endDate")}
                  className="w-full rounded border px-2 py-1.5 text-xs"
                />
              </div>
            </div>
            <div>
              <label className="text-xs font-medium">Answer 1</label>
              <input
                {...pollForm.register("answers.0")}
                className="w-full rounded border px-3 py-1 text-xs"
              />
            </div>
            <div>
              <label className="text-xs font-medium">Answer 2</label>
              <input
                {...pollForm.register("answers.1")}
                className="w-full rounded border px-3 py-1 text-xs"
              />
            </div>
            <button
              type="submit"
              disabled={isCreatingPoll}
              className="w-full rounded bg-slate-900 py-2 text-sm text-white"
            >
              {isCreatingPoll ? "Creating..." : "Create Poll"}
            </button>
          </form>
        </div>
      </div>

      {/* Control Elections & Polls statuses */}
      <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
        <div className="rounded-xl border border-slate-200 bg-white p-6 shadow-xs">
          <h3 className="mb-3 text-md font-semibold">
            Manage Elections Status
          </h3>
          <div className="space-y-2 text-xs">
            {elections?.length === 0 && (
              <p className="text-xs text-slate-400">No elections found.</p>
            )}
          </div>
        </div>

        <div className="rounded-xl border border-slate-200 bg-white p-6 shadow-xs">
          <h3 className="mb-3 text-md font-semibold">Manage Polls Status</h3>
          <div className="space-y-2 text-xs">
            {polls?.length === 0 && (
              <p className="text-xs text-slate-400">No polls found.</p>
            )}
          </div>
        </div>
      </div>

      {/* Audit Logs table */}
      <div className="rounded-xl border border-slate-200 bg-white p-6 shadow-xs">
        <h3 className="mb-4 text-lg font-semibold">System Audit Logs</h3>
        <table className="w-full text-left text-xs">
          <thead className="border-b uppercase text-slate-400">
            <tr>
              <th className="pb-2">Timestamp</th>
              <th className="pb-2">Admin</th>
              <th className="pb-2">Action Type</th>
              <th className="pb-2">Details</th>
            </tr>
          </thead>
          <tbody className="divide-y">
            {auditLogs?.length === 0 && (
              <tr>
                <td className="py-2" colSpan={4}>
                  <p className="text-xs text-slate-400">No audit logs found.</p>
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default AdminPage;
