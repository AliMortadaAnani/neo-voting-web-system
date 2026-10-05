import type { Role } from "../types/auth.type";

export type UserClaims = {
  role: Role;
  appUserId: number;
  accountId?: number;
  username: string;
};

// Map each role to its dedicated home portal
export const ROLE_HOME: Record<Role, string> = {
  Admin: "/admin",
  Voter: "/voter",
  Candidate: "/candidate",
};

export const decodeUser = (token: string): UserClaims | null => {
  try {
    const parts = token.split(".");
    if (parts.length !== 3) return null;

    // Base64Url -> Base64 -> UTF-8 JSON
    const base64Url = parts[1];
    const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split("")
        .map((c) => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
        .join(""),
    );

    const payload = JSON.parse(jsonPayload);

    // 1. Role (checks standard or Microsoft claim URI)
    const role = (payload["role"] ||
      payload[
        "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
      ]) as Role;

    // 2. Username
    const username =
      payload["unique_name"] ||
      payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] ||
      payload["name"] ||
      "";

    // 3. Application User ID
    const rawUserId =
      payload["applicationUserId"] ||
      payload[
        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
      ] ||
      payload["nameid"];

    // Role and User ID are strictly required
    if (!role || !rawUserId) {
      return null;
    }

    return {
      role,
      appUserId: Number(rawUserId),
      accountId: payload["accountId"]
        ? Number(payload["accountId"])
        : undefined,
      username,
    };
  } catch {
    return null;
  }
};
