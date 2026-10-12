export type CandidateProfileResponseDTO = {
  candidateProfileId: number;
  candidateAccountId: number;
  firstName: string;
  lastName: string;
  gender: string;
  dateOfBirth: string;
  nominationReasons: string;
  votesCount?: number;
};

export type PollAnswerDTO = {
  id: number;
  answer: string;
  votesCount?: number;
};

export type PollResponseDTO = {
  id: number;
  name: string;
  question: string;
  startDate: string;
  endDate: string;
  status: string;
  answers: PollAnswerDTO[];
};
