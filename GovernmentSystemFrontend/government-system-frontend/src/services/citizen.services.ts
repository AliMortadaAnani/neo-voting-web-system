import { api } from "../api/api";
import type {
  CitizenResponseDTO,
  PagedResult,
  CitizenCreateRequestDTO,
  CitizenFetchRequestDTO,
  CitizenUpdateRequestDTO,
} from "../types/citizen.types";

export const getCitizensPaged = async (
  pageNumber = 1,
  pageSize = 10,
): Promise<PagedResult<CitizenResponseDTO>> => {
  const res = await api.get<PagedResult<CitizenResponseDTO>>(
    "/Citizens/paged",
    {
      params: { pageNumber, pageSize },
    },
  );
  return res.data;
};

export const getCitizenDetails = async (
  dto: CitizenFetchRequestDTO,
): Promise<CitizenResponseDTO> => {
  const res = await api.post<CitizenResponseDTO>("/Citizens/details", dto);
  return res.data;
};

export const createCitizen = async (
  dto: CitizenCreateRequestDTO,
): Promise<CitizenResponseDTO> => {
  const res = await api.post<CitizenResponseDTO>("/Citizens/add", dto);
  return res.data;
};

export const updateCitizen = async (
  dto: CitizenUpdateRequestDTO,
): Promise<CitizenResponseDTO> => {
  const res = await api.put<CitizenResponseDTO>("/Citizens/updateDetails", dto);
  return res.data;
};

export const deleteCitizen = async (
  dto: CitizenFetchRequestDTO,
): Promise<boolean> => {
  const res = await api.post<boolean>("/Citizens/delete", dto);
  return res.data;
};

export const getCitizensTotalCount = async (): Promise<number> => {
  const res = await api.get<number>("/Citizens/totalCount");
  return res.data;
};
