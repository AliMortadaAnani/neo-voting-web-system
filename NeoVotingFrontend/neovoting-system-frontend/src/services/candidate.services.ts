import { api } from "../api/api";
import type { CandidateProfileCreateRequestDTO } from "../types/candidate.types";
import type { CandidateProfileResponseDTO } from "../types/general.types";

export const createCandidateProfile = async (
  electionId: number,
  dto: CandidateProfileCreateRequestDTO,
): Promise<CandidateProfileResponseDTO> => {
  const response = await api.post<CandidateProfileResponseDTO>(
    `/Candidate/elections/${electionId}/profiles`,
    dto,
  );
  return response.data;
};
