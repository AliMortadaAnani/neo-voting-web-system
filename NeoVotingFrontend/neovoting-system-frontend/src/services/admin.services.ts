import { api } from "../api/api";
import type {
  ElectionCreateRequestDTO,
  ElectionResponseDTO,
  SystemAuditLogResponseDTO,
} from "../types/admin.types";
import type { PollResponseDTO } from "../types/general.types";
import type { PollCreateRequestDTO } from "../types/admin.types";

export const createElection = async (
  dto: ElectionCreateRequestDTO,
): Promise<ElectionResponseDTO> => {
  const response = await api.post<ElectionResponseDTO>("/Admin/elections", dto);
  return response.data;
};

export const createPoll = async (
  dto: PollCreateRequestDTO,
): Promise<PollResponseDTO> => {
  const response = await api.post<PollResponseDTO>("/Admin/polls", dto);
  return response.data;
};

export const startElection = async (
  electionId: number,
): Promise<ElectionResponseDTO> => {
  const response = await api.patch<ElectionResponseDTO>(
    `/Admin/elections/${electionId}/start`,
  );
  return response.data;
};

export const completeElection = async (
  electionId: number,
): Promise<ElectionResponseDTO> => {
  const response = await api.patch<ElectionResponseDTO>(
    `/Admin/elections/${electionId}/complete`,
  );
  return response.data;
};

export const startPoll = async (pollId: number): Promise<PollResponseDTO> => {
  const response = await api.patch<PollResponseDTO>(
    `/Admin/polls/${pollId}/start`,
  );
  return response.data;
};

export const completePoll = async (
  pollId: number,
): Promise<PollResponseDTO> => {
  const response = await api.patch<PollResponseDTO>(
    `/Admin/polls/${pollId}/complete`,
  );
  return response.data;
};

export const getPagedSystemAuditLogs = async (
  actionTypesEnum?: number,
  adminId?: number,
  pageNumber = 1,
  pageSize = 10,
): Promise<SystemAuditLogResponseDTO[]> => {
  const response = await api.get<SystemAuditLogResponseDTO[]>(
    "/Admin/systemAuditLogs",
    {
      params: { actionTypesEnum, adminId, pageNumber, pageSize },
    },
  );
  return response.data;
};
