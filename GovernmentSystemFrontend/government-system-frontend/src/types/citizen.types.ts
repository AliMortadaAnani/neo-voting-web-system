export const Governorate = {
  Beirut: 1,
  MountLebanon: 2,
  South: 3,
  East: 4,
  North: 5,
} as const;

export type Governorate = (typeof Governorate)[keyof typeof Governorate];

export const GOVERNORATE_NAMES: Record<number, string> = {
  [Governorate.Beirut]: "Beirut",
  [Governorate.MountLebanon]: "Mount Lebanon",
  [Governorate.South]: "South",
  [Governorate.East]: "East",
  [Governorate.North]: "North",
};

export type CitizenResponse = {
  id: number;
  nationalId: string;
  firstName: string;
  lastName: string;
  dateOfBirth: string; // ISO string YYYY-MM-DD
  governorate: Governorate;
  gender: "M" | "F" | string;
};

export type PagedResult<T> = {
  data: T[];
  currentPage: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
};

export type CreateCitizenDTO = {
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  governorate: number;
  gender: string;
};

export type UpdateCitizenDTO = CreateCitizenDTO & {
  nationalId: string;
};
