using NeoVoting.Application.RequestDTOs.CandidateDTOs;
using NeoVoting.Application.ResponseDTOs.GeneralDTOs;
using NeoVoting.Domain.ResultErrorDomain;

namespace NeoVoting.Application.ServicesContracts
{
    public interface ICandidateServices
    {
        Task<Result<CandidateProfile_ResponseDTO>> CreateCandidateProfileAsync(int electionId, CandidateProfile_Create_RequestDTO candidateRequestDTO);
    }
}