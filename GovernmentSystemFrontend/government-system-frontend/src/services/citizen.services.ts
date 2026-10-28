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
  console.log("[Citizens] Loading citizen list", { pageNumber, pageSize });
  const res = await api.get<PagedResult<CitizenResponseDTO>>(
    "/Citizens/paged",
    {
      params: { pageNumber, pageSize },
    },
  );
  console.log("[Citizens] Citizen list loaded, response:", res.data);
  return res.data;
};

export const getCitizenDetails = async (
  dto: CitizenFetchRequestDTO,
): Promise<CitizenResponseDTO> => {
  console.log("[Citizens] Loading citizen details:", dto);
  const res = await api.post<CitizenResponseDTO>("/Citizens/details", dto);
  console.log("[Citizens] Citizen details loaded, response:", res.data);
  return res.data;
};

export const createCitizen = async (
  dto: CitizenCreateRequestDTO,
): Promise<CitizenResponseDTO> => {
  console.log("[Citizens] Creating citizen:", dto);
  const res = await api.post<CitizenResponseDTO>("/Citizens/add", dto);
  console.log("[Citizens] Citizen created, response:", res.data);
  return res.data;
};

export const updateCitizen = async (
  dto: CitizenUpdateRequestDTO,
): Promise<CitizenResponseDTO> => {
  console.log("[Citizens] Updating citizen:", dto);
  const res = await api.put<CitizenResponseDTO>("/Citizens/updateDetails", dto);
  console.log("[Citizens] Citizen updated, response:", res.data);
  return res.data;
};

export const deleteCitizen = async (
  dto: CitizenFetchRequestDTO,
): Promise<boolean> => {
  console.log("[Citizens] Deleting citizen:", dto);
  const res = await api.post<boolean>("/Citizens/delete", dto);
  console.log("[Citizens] Citizen deleted, response:", res.data);
  return res.data;
};

export const getCitizensTotalCount = async (): Promise<number> => {
  console.log("[Citizens] Loading citizen count");
  const res = await api.get<number>("/Citizens/totalCount");
  console.log("[Citizens] Citizen count loaded, response:", res.data);
  return res.data;
};
