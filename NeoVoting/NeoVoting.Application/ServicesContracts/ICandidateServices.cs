using Microsoft.AspNetCore.Http;
using NeoVoting.Application.RequestDTOs;
using NeoVoting.Application.ResponseDTOs;
using NeoVoting.Domain.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.ServicesContracts
{
    public interface ICandidateServices
    {
        Task<Result<CandidateProfile_ResponseDTO>> AddCandidateProfileByElectionIdAsync(Guid electionId,CandidateProfileAdd_RequestDTO request, CancellationToken cancellationToken);


        Task<Result<CandidateProfile_ResponseDTO>> UpdateCandidateProfileByElectionIdAsync(Guid electionId,CandidateProfileUpdate_RequestDTO request, CancellationToken cancellationToken);


        Task<Result<CandidateProfile_ResponseDTO>> GetCandidateProfileByElectionIdAsync(Guid electionId,CancellationToken cancellationToken);

        // Returns the new URL
        Task<Result<string>> UpdateCandidateProfile_Photo_ByElectionIdAsync(
            Guid electionId,
            CandidateProfileUploadImage_RequestDTO request , CancellationToken cancellationToken);

    }
}
