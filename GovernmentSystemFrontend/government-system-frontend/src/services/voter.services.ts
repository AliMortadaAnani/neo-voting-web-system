import { api } from "../api/api";
import type {
  VoterResponse,
  PagedResult,
  CreateVoterDTO,
  UpdateVoterDTO,
} from "../types/voter.types";

export const getVotersPaged = async (
  pageNumber = 1,
  pageSize = 10,
): Promise<PagedResult<VoterResponse>> => {
  const response = await api.get<PagedResult<VoterResponse>>("/Voters/paged", {
    params: { pageNumber, pageSize },
  });
  return response.data;
};

export const getVoterDetails = async (
  nationalId: string,
): Promise<VoterResponse> => {
  const response = await api.post<VoterResponse>("/Voters/details", {
    nationalId,
  });
  return response.data;
};

export const addVoter = async (dto: CreateVoterDTO): Promise<VoterResponse> => {
  const response = await api.post<VoterResponse>("/Voters/add", dto);
  return response.data;
};

export const regenerateVotingToken = async (
  dto: UpdateVoterDTO,
): Promise<VoterResponse> => {
  const response = await api.put<VoterResponse>(
    "/Voters/generateNewToken",
    dto,
  );
  return response.data;
};

export const deleteVoter = async (nationalId: string): Promise<boolean> => {
  const response = await api.post<boolean>("/Voters/delete", { nationalId });
  return response.data;
};

export const getVotersTotalCount = async (): Promise<number> => {
  const response = await api.get<number>("/Voters/totalCount");
  return response.data;
};
