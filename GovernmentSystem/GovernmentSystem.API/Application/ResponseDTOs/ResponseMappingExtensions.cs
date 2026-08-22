using GovernmentSystem.API.Domain.Entities;
using GovernmentSystem.API.Application.ResponseDTOs.VoterDTOs;
using GovernmentSystem.API.Application.ResponseDTOs.CandidateDTOs;
using GovernmentSystem.API.Application.ResponseDTOs.CitizenDTOs;

namespace GovernmentSystem.API.Application.ResponseDTOs
{
    public static class ResponseMappingExtensions
    {
        public static VoterResponseDTO ToVoterResponse(this Voter voter)
        {
            return new VoterResponseDTO
            {
                Id = voter.Id,
                CitizenId = voter.CitizenId,
                VotingToken = voter.VotingToken,
                HashedData = voter.HashedData,
                NationalId = voter.Citizen.NationalId,
                GovernorateId = voter.Citizen.GovernorateId,
                FirstName = voter.Citizen.FirstName,
                LastName = voter.Citizen.LastName,
                DateOfBirth = voter.Citizen.DateOfBirth,
                Gender = voter.Citizen.Gender
            };
        }

        public static VoterVerifyResponseDTO ToNeoVoting_VoterResponse(this Voter voter)
        {
            return new VoterVerifyResponseDTO
            {
                HashedData = voter.HashedData,
                GovernorateId = voter.Citizen.GovernorateId,
                FirstName = voter.Citizen.FirstName,
                LastName = voter.Citizen.LastName,
                DateOfBirth = voter.Citizen.DateOfBirth,
                Gender = voter.Citizen.Gender
            };
        }

        // --- Candidate Mappings ---

        public static CandidateResponseDTO ToCandidateResponse(this Candidate candidate)
        {
            return new CandidateResponseDTO
            {
                Id = candidate.Id,
                CitizenId = candidate.CitizenId,
                NominationToken = candidate.NominationToken,
                HashedData = candidate.HashedData,
                NationalId = candidate.Citizen.NationalId,
                GovernorateId = candidate.Citizen.GovernorateId,
                FirstName = candidate.Citizen.FirstName,
                LastName = candidate.Citizen.LastName,
                DateOfBirth = candidate.Citizen.DateOfBirth,
                Gender = candidate.Citizen.Gender
            };
        }

        public static CandidateVerifyResponseDTO ToNeoVoting_CandidateResponse(this Candidate candidate)
        {
            return new CandidateVerifyResponseDTO
            {
                HashedData = candidate.HashedData,
                GovernorateId = candidate.Citizen.GovernorateId,
                FirstName = candidate.Citizen.FirstName,
                LastName = candidate.Citizen.LastName,
                DateOfBirth = candidate.Citizen.DateOfBirth,
                Gender = candidate.Citizen.Gender
            };
        }
    
        
    }
}