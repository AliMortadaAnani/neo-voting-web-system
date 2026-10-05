import { RegisterForm } from "../components/RegisterForm";
import { registerVoter } from "../services/auth.services";

export const RegisterVoterPage = () => {
  return (
    <RegisterForm
      roleTitle="Voter"
      tokenLabel="Voting Token"
      submitFn={registerVoter}
    />
  );
};

export default RegisterVoterPage;
