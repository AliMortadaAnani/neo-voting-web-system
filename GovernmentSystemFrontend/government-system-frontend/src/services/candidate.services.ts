import { api } from "../api/api";
import type {
  CandidateResponseDTO,
  PagedResult,
  CandidateRequestDTO,
} from "../types/candidate.types";

export const getCandidatesPaged = async (
  pageNumber = 1,
  pageSize = 10,
): Promise<PagedResult<CandidateResponseDTO>> => {
  console.log("[Candidates] Loading candidate list", { pageNumber, pageSize });
  const res = await api.get<PagedResult<CandidateResponseDTO>>(
    "/Candidates/paged",
    { params: { pageNumber, pageSize } },
  );
  console.log("[Candidates] Candidate list loaded, response:", res.data);
  return res.data;
};

export const getCandidateDetails = async (
  dto: CandidateRequestDTO,
): Promise<CandidateResponseDTO> => {
  console.log("[Candidates] Loading candidate details:", dto);
  const res = await api.post<CandidateResponseDTO>("/Candidates/details", dto);
  console.log("[Candidates] Candidate details loaded, response:", res.data);
  return res.data;
};

export const addCandidate = async (
  dto: CandidateRequestDTO,
): Promise<CandidateResponseDTO> => {
  console.log("[Candidates] Adding candidate:", dto);
  const res = await api.post<CandidateResponseDTO>("/Candidates/add", dto);
  console.log("[Candidates] Candidate added, response:", res.data);
  return res.data;
};

export const regenerateNominationToken = async (
  dto: CandidateRequestDTO,
): Promise<CandidateResponseDTO> => {
  console.log("[Candidates] Generating nomination token:", dto);
  const res = await api.put<CandidateResponseDTO>(
    "/Candidates/generateNewToken",
    dto,
  );
  console.log("[Candidates] Nomination token generated, response:", res.data);
  return res.data;
};

export const deleteCandidate = async (
  dto: CandidateRequestDTO,
): Promise<boolean> => {
  console.log("[Candidates] Deleting candidate:", dto);
  const res = await api.post<boolean>("/Candidates/delete", dto);
  console.log("[Candidates] Candidate deleted, response:", res.data);
  return res.data;
};

export const getCandidatesTotalCount = async (): Promise<number> => {
  console.log("[Candidates] Loading candidate count");
  const res = await api.get<number>("/Candidates/totalCount");
  console.log("[Candidates] Candidate count loaded, response:", res.data);
  return res.data;
};
