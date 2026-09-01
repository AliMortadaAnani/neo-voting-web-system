using NeoVoting.Application.RequestDTOs.CandidateDTOs;
using NeoVoting.Application.ResponseDTOs.CandidateDTOs;
using NeoVoting.Domain.ResultErrorDomain;

namespace NeoVoting.Application.ServicesContracts
{
    public interface ICandidateServices
    {
       Task<Result<CandidateProfile_ResponseDTO>> CreateCandidateProfileAsync(int electionId, CandidateProfile_Create_Update_RequestDTO candidateRequestDTO);

        Task<Result<CandidateProfile_ResponseDTO>> GetCandidateProfileAsync(int electionId);

        Task<Result<CandidateProfile_ResponseDTO>> UpdateCandidateProfileAsync(int electionId, CandidateProfile_Create_Update_RequestDTO candidateRequestDTO);

        Task<Result<CandidateProfile_ResponseDTO>> UpdateImageForCandidateProfileAsync(int electionId, CandidateProfileUploadImage_RequestDTO candidateRequestDTO);

        Task<Result<CandidateProfile_ResponseDTO>> RemoveImageForCandidateProfileAsync(int electionId);

    }
}