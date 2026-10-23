import { useNavigate } from "react-router-dom";

export function useGoBack(fallback: number = -1) {
  const navigate = useNavigate();

  return () => {
    navigate(fallback);
  };
}
