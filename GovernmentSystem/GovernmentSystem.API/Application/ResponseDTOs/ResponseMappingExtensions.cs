using GovernmentSystem.API.Domain.Entities;

namespace GovernmentSystem.API.Application.ResponseDTOs
{
    public static class ResponseMappingExtensions
    {
        public static VoterResponseDTO ToVoterResponse(this Voter voter)
        {
            return new VoterResponseDTO
            {
                Id = voter.Id,
                NationalId = voter.NationalId,
                VotingToken = voter.VotingToken,
                GovernorateId = voter.GovernorateId,
                FirstName = voter.FirstName,
                LastName = voter.LastName,
                DateOfBirth = voter.DateOfBirth,
                Gender = voter.Gender,
                EligibleForElection = voter.EligibleForElection,
                ValidToken = voter.ValidToken,
                IsRegistered = voter.IsRegistered,
                Voted = voter.Voted
            };
        }

        public static NeoVoting_VoterResponseDTO ToNeoVoting_VoterResponse(this Voter voter)
        {
            return new NeoVoting_VoterResponseDTO
            {
                GovernorateId = voter.GovernorateId,
                FirstName = voter.FirstName,
                LastName = voter.LastName,
                DateOfBirth = voter.DateOfBirth,
                Gender = voter.Gender,
                EligibleForElection = voter.EligibleForElection,
                ValidToken = voter.ValidToken,
                IsRegistered = voter.IsRegistered,
                Voted = voter.Voted
            };
        }

        // --- Candidate Mappings ---

        public static CandidateResponseDTO ToCandidateResponse(this Candidate candidate)
        {
            return new CandidateResponseDTO
            {
                Id = candidate.Id,
                NationalId = candidate.NationalId,
                NominationToken = candidate.NominationToken,
                GovernorateId = candidate.GovernorateId,
                FirstName = candidate.FirstName,
                LastName = candidate.LastName,
                DateOfBirth = candidate.DateOfBirth,
                Gender = candidate.Gender,
                EligibleForElection = candidate.EligibleForElection,
                ValidToken = candidate.ValidToken,
                IsRegistered = candidate.IsRegistered
            };
        }

        public static NeoVoting_CandidateResponseDTO ToNeoVoting_CandidateResponse(this Candidate candidate)
        {
            return new NeoVoting_CandidateResponseDTO
            {
                GovernorateId = candidate.GovernorateId,
                FirstName = candidate.FirstName,
                LastName = candidate.LastName,
                DateOfBirth = candidate.DateOfBirth,
                Gender = candidate.Gender,
                EligibleForElection = candidate.EligibleForElection,
                ValidToken = candidate.ValidToken,
                IsRegistered = candidate.IsRegistered
            };
        }
    }
}
