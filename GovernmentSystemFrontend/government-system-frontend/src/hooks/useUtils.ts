import { useNavigate, useLocation } from "react-router-dom";

export function useGoBack(fallback: number = -1) {
  const navigate = useNavigate();
  const location = useLocation();

  return () => {
    if (location.state?.from) {
      navigate(location.state.from);
    } else {
      navigate(fallback);
    }
  };
}
