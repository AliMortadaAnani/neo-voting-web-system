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
  console.log("[Voters] Loading voter list", { pageNumber, pageSize });
  const res = await api.get<PagedResult<VoterResponseDTO>>("/Voters/paged", {
    params: { pageNumber, pageSize },
  });
  console.log("[Voters] Voter list loaded, response:", res.data);
  return res.data;
};

export const getVoterDetails = async (
  dto: VoterRequestDTO,
): Promise<VoterResponseDTO> => {
  console.log("[Voters] Loading voter details:", dto);
  const res = await api.post<VoterResponseDTO>("/Voters/details", dto);
  console.log("[Voters] Voter details loaded, response:", res.data);
  return res.data;
};

export const addVoter = async (
  dto: VoterRequestDTO,
): Promise<VoterResponseDTO> => {
  console.log("[Voters] Adding voter:", dto);
  const res = await api.post<VoterResponseDTO>("/Voters/add", dto);
  console.log("[Voters] Voter added, response:", res.data);
  return res.data;
};

export const regenerateVotingToken = async (
  dto: VoterRequestDTO,
): Promise<VoterResponseDTO> => {
  console.log("[Voters] Generating voting token:", dto);
  const res = await api.put<VoterResponseDTO>("/Voters/generateNewToken", dto);
  console.log("[Voters] Voting token generated, response:", res.data);
  return res.data;
};

export const deleteVoter = async (dto: VoterRequestDTO): Promise<boolean> => {
  console.log("[Voters] Deleting voter:", dto);
  const res = await api.post<boolean>("/Voters/delete", dto);
  console.log("[Voters] Voter deleted, response:", res.data);
  return res.data;
};

export const getVotersTotalCount = async (): Promise<number> => {
  console.log("[Voters] Loading voter count");
  const res = await api.get<number>("/Voters/totalCount");
  console.log("[Voters] Voter count loaded, response:", res.data);
  return res.data;
};
