using NeoVoting.Application.RequestDTOs.CandidateDTOs;
using NeoVoting.Application.ResponseDTOs.GeneralDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.ResultErrorDomain;

namespace NeoVoting.Application.Services
{
    public class CandidateServices : ICandidateServices
    {
        public Task<Result<CandidateProfile_ResponseDTO>> CreateCandidateProfileAsync(int electionId, CandidateProfile_Create_RequestDTO candidateRequestDTO)
        {
            throw new NotImplementedException();
        }
    }
}