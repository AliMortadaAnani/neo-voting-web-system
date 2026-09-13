import axios from "axios";

export const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  withCredentials: true, // Crucial: Ensures cookies (sessions/JWTs) are sent/received automatically
  headers: {
    "Content-Type": "application/json",
  },
});
