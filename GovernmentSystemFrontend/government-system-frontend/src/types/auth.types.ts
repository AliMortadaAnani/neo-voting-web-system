export interface AuthResponseDto {
  Username: string;
  Role: string;
  Message: string;
  IsSuccess: boolean;
}

export interface LoginRequestDto {
  Username: string;
  Password: string;
}
