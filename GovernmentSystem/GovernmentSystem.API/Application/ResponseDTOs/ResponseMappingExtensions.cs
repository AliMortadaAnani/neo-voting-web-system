using GovernmentSystem.API.Application.Helpers;
using GovernmentSystem.API.Application.ResponseDTOs.CandidateDTOs;
using GovernmentSystem.API.Application.ResponseDTOs.CitizenDTOs;
using GovernmentSystem.API.Application.ResponseDTOs.VoterDTOs;
using GovernmentSystem.API.Domain.Entities;

namespace GovernmentSystem.API.Application.ResponseDTOs
{
    public static class ResponseMappingExtensions
    {
        public static CitizenResponseDTO ToCitizenResponse(this Citizen citizen, SensitiveDataHelper? sensitiveDataHelper = null)
        {
            return new CitizenResponseDTO
            {
                Id = citizen.Id,
                NationalId = sensitiveDataHelper?.Decrypt(citizen.NationalId) ?? citizen.NationalId,
                Governorate = citizen.Governorate,
                FirstName = citizen.FirstName,
                LastName = citizen.LastName,
                DateOfBirth = citizen.DateOfBirth,
                Gender = citizen.Gender
            };
        }

        public static VoterResponseDTO ToVoterResponse(this Voter voter, SensitiveDataHelper? sensitiveDataHelper = null)
        {
            return new VoterResponseDTO
            {
                Id = voter.Id,
                CitizenId = voter.CitizenId,
                VotingToken = sensitiveDataHelper?.Decrypt(voter.VotingToken) ?? voter.VotingToken,
                HashedData = voter.HashedData,
                NationalId = sensitiveDataHelper?.Decrypt(voter.Citizen.NationalId) ?? voter.Citizen.NationalId,
                Governorate = voter.Citizen.Governorate,
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
                Governorate = voter.Citizen.Governorate,
                FirstName = voter.Citizen.FirstName,
                LastName = voter.Citizen.LastName,
                DateOfBirth = voter.Citizen.DateOfBirth,
                Gender = voter.Citizen.Gender
            };
        }

        // --- Candidate Mappings ---

        public static CandidateResponseDTO ToCandidateResponse(this Candidate candidate, SensitiveDataHelper? sensitiveDataHelper = null)
        {
            return new CandidateResponseDTO
            {
                Id = candidate.Id,
                CitizenId = candidate.CitizenId,
                NominationToken = sensitiveDataHelper?.Decrypt(candidate.NominationToken) ?? candidate.NominationToken,
                HashedData = candidate.HashedData,
                NationalId = sensitiveDataHelper?.Decrypt(candidate.Citizen.NationalId) ?? candidate.Citizen.NationalId,
                Governorate = candidate.Citizen.Governorate,
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
                Governorate = candidate.Citizen.Governorate,
                FirstName = candidate.Citizen.FirstName,
                LastName = candidate.Citizen.LastName,
                DateOfBirth = candidate.Citizen.DateOfBirth,
                Gender = candidate.Citizen.Gender
            };
        }
    }
}