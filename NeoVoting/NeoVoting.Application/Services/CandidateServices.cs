using NeoVoting.Application.RequestDTOs.CandidateDTOs;
using NeoVoting.Application.ResponseDTOs.CandidateDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Domain.ResultErrorDomain;

namespace NeoVoting.Application.Services
{
    public class CandidateServices : ICandidateServices
    {
        public Task<Result<CandidateProfile_ResponseDTO>> CreateCandidateProfileAsync(int electionId, CandidateProfile_Create_Update_RequestDTO candidateRequestDTO)
        {
            throw new NotImplementedException();
        }

        public Task<Result<CandidateProfile_ResponseDTO>> GetCandidateProfileAsync(int electionId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<CandidateProfile_ResponseDTO>> RemoveImageForCandidateProfileAsync(int electionId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<CandidateProfile_ResponseDTO>> UpdateCandidateProfileAsync(int electionId, CandidateProfile_Create_Update_RequestDTO candidateRequestDTO)
        {
            throw new NotImplementedException();
        }

        public Task<Result<CandidateProfile_ResponseDTO>> UpdateImageForCandidateProfileAsync(int electionId, CandidateProfileUploadImage_RequestDTO candidateRequestDTO)
        {
            throw new NotImplementedException();
        }
    }

}