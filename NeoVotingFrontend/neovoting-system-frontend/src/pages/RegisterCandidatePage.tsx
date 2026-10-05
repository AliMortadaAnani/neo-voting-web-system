import { RegisterForm } from "../components/RegisterForm";
import { registerCandidate } from "../services/auth.services";

export const RegisterCandidatePage = () => {
  return (
    <RegisterForm
      roleTitle="Candidate"
      tokenLabel="Nomination Token"
      submitFn={registerCandidate}
    />
  );
};

export default RegisterCandidatePage;
