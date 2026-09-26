import { api } from "../api/api";
import type {
  CandidateResponse,
  PagedResult,
  CreateCandidateDTO,
  UpdateCandidateDTO,
} from "../types/candidate.types";

export const getCandidatesPaged = async (
  pageNumber = 1,
  pageSize = 10,
): Promise<PagedResult<CandidateResponse>> => {
  const response = await api.get<PagedResult<CandidateResponse>>(
    "/Candidates/paged",
    {
      params: { pageNumber, pageSize },
    },
  );
  return response.data;
};

export const getCandidateDetails = async (
  nationalId: string,
): Promise<CandidateResponse> => {
  const response = await api.post<CandidateResponse>("/Candidates/details", {
    nationalId,
  });
  return response.data;
};

export const addCandidate = async (
  dto: CreateCandidateDTO,
): Promise<CandidateResponse> => {
  const response = await api.post<CandidateResponse>("/Candidates/add", dto);
  return response.data;
};

export const regenerateNominationToken = async (
  dto: UpdateCandidateDTO,
): Promise<CandidateResponse> => {
  const response = await api.put<CandidateResponse>(
    "/Candidates/generateNewToken",
    dto,
  );
  return response.data;
};

export const deleteCandidate = async (nationalId: string): Promise<boolean> => {
  const response = await api.post<boolean>("/Candidates/delete", {
    nationalId,
  });
  return response.data;
};

export const getCandidatesTotalCount = async (): Promise<number> => {
  const response = await api.get<number>("/Candidates/totalCount");
  return response.data;
};
