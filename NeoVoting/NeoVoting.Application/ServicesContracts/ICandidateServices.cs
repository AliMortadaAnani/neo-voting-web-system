using NeoVoting.Application.RequestDTOs;
using NeoVoting.Application.ResponseDTOs;
using NeoVoting.Domain.ResultErrorDomain;

namespace NeoVoting.Application.ServicesContracts
{
    public interface ICandidateServices
    {
        Task<Result<CandidateProfile_ResponseDTO>> AddCandidateProfileByElectionIdAsync(Guid electionId, CandidateProfileAdd_RequestDTO request, CancellationToken cancellationToken);

        Task<Result<CandidateProfile_ResponseDTO>> UpdateCandidateProfileByElectionIdAsync(Guid electionId, CandidateProfileUpdate_RequestDTO request, CancellationToken cancellationToken);

        Task<Result<CandidateProfile_ResponseDTO>> GetCandidateProfileByElectionIdAsync(Guid electionId, CancellationToken cancellationToken);

        // Returns the new URL
        Task<Result<string>> UpdateCandidateProfile_Photo_ByElectionIdAsync(
            Guid electionId,
            CandidateProfileUploadImage_RequestDTO request, CancellationToken cancellationToken);
    }
}