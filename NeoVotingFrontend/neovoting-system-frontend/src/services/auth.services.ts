import { api } from "../api/api";
import type {
  AuthResponseDTO,
  RegisterVoterOrCandidateResponseDTO,
  LoginRequestDTO,
  RegisterVoterOrCandidateRequestDTO,
} from "../types/auth.types";

export const login = async (
  payload: LoginRequestDTO,
): Promise<AuthResponseDTO> => {
  return (await api.post("/Auth/login", payload)).data;
};

export const logout = async (): Promise<void> => {
  return (await api.post("/Auth/logout")).data;
};

export const registerVoter = async (
  payload: RegisterVoterOrCandidateRequestDTO,
): Promise<RegisterVoterOrCandidateResponseDTO> => {
  return (await api.post("/Auth/register-voter", payload)).data;
};

export const registerCandidate = async (
  payload: RegisterVoterOrCandidateRequestDTO,
): Promise<RegisterVoterOrCandidateResponseDTO> => {
  return (await api.post("/Auth/register-candidate", payload)).data;
};

// --- Role Verification Endpoints ---

export const testAdmin = async (): Promise<string> => {
  return api.get("/Auth/test-admin");
};

export const testVoter = async (): Promise<string> => {
  return api.get("/Auth/test-voter");
};

export const testCandidate = async (): Promise<string> => {
  return api.get("/Auth/test-candidate");
};
