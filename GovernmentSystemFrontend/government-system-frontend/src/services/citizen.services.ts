import { api } from "../api/api";
import type {
  CitizenResponse,
  PagedResult,
  CreateCitizenDTO,
  UpdateCitizenDTO,
} from "../types/citizen.types";

export const getCitizensPaged = async (
  pageNumber = 1,
  pageSize = 10,
): Promise<PagedResult<CitizenResponse>> => {
  const response = await api.get<PagedResult<CitizenResponse>>(
    "/Citizens/paged",
    {
      params: { pageNumber, pageSize },
    },
  );
  return response.data;
};

export const getCitizenDetails = async (
  nationalId: string,
): Promise<CitizenResponse> => {
  const response = await api.post<CitizenResponse>("/Citizens/details", {
    nationalId,
  });
  return response.data;
};

export const createCitizen = async (
  dto: CreateCitizenDTO,
): Promise<CitizenResponse> => {
  const response = await api.post<CitizenResponse>("/Citizens/add", dto);
  return response.data;
};

export const updateCitizen = async (
  dto: UpdateCitizenDTO,
): Promise<CitizenResponse> => {
  const response = await api.put<CitizenResponse>(
    "/Citizens/updateDetails",
    dto,
  );
  return response.data;
};

export const deleteCitizen = async (nationalId: string): Promise<boolean> => {
  const response = await api.post<boolean>("/Citizens/delete", { nationalId });
  return response.data;
};

export const getCitizensTotalCount = async (): Promise<number> => {
  const response = await api.get<number>("/Citizens/totalCount");
  return response.data;
};
