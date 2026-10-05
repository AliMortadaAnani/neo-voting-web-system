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
  const res = await api.get<PagedResult<CandidateResponseDTO>>(
    "/Candidates/paged",
    { params: { pageNumber, pageSize } },
  );
  return res.data;
};

export const getCandidateDetails = async (
  dto: CandidateRequestDTO,
): Promise<CandidateResponseDTO> => {
  const res = await api.post<CandidateResponseDTO>("/Candidates/details", dto);
  return res.data;
};

export const addCandidate = async (
  dto: CandidateRequestDTO,
): Promise<CandidateResponseDTO> => {
  const res = await api.post<CandidateResponseDTO>("/Candidates/add", dto);
  return res.data;
};

export const regenerateNominationToken = async (
  dto: CandidateRequestDTO,
): Promise<CandidateResponseDTO> => {
  const res = await api.put<CandidateResponseDTO>(
    "/Candidates/generateNewToken",
    dto,
  );
  return res.data;
};

export const deleteCandidate = async (
  dto: CandidateRequestDTO,
): Promise<boolean> => {
  const res = await api.post<boolean>("/Candidates/delete", dto);
  return res.data;
};

export const getCandidatesTotalCount = async (): Promise<number> => {
  const res = await api.get<number>("/Candidates/totalCount");
  return res.data;
};
