import { api } from "../api/api";
import type {
  VoterResponseDTO,
  PagedResult,
  VoterRequestDTO,
} from "../types/voter.types";

export const getVotersPaged = async (
  pageNumber = 1,
  pageSize = 10,
): Promise<PagedResult<VoterResponseDTO>> => {
  const res = await api.get<PagedResult<VoterResponseDTO>>("/Voters/paged", {
    params: { pageNumber, pageSize },
  });
  return res.data;
};

export const getVoterDetails = async (
  dto: VoterRequestDTO,
): Promise<VoterResponseDTO> => {
  const res = await api.post<VoterResponseDTO>("/Voters/details", dto);
  return res.data;
};

export const addVoter = async (
  dto: VoterRequestDTO,
): Promise<VoterResponseDTO> => {
  const res = await api.post<VoterResponseDTO>("/Voters/add", dto);
  return res.data;
};

export const regenerateVotingToken = async (
  dto: VoterRequestDTO,
): Promise<VoterResponseDTO> => {
  const res = await api.put<VoterResponseDTO>("/Voters/generateNewToken", dto);
  return res.data;
};

export const deleteVoter = async (dto: VoterRequestDTO): Promise<boolean> => {
  const res = await api.post<boolean>("/Voters/delete", dto);
  return res.data;
};

export const getVotersTotalCount = async (): Promise<number> => {
  const res = await api.get<number>("/Voters/totalCount");
  return res.data;
};
