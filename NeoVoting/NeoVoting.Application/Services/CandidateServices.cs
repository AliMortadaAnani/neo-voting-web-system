using NeoVoting.Application.RequestDTOs;
using NeoVoting.Application.ResponseDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Services
{
    public class CandidateServices : ICandidateServices
    {
        public Task<Result<CandidateProfile_ResponseDTO>> AddCandidateProfileByElectionIdAsync(Guid electionId, CandidateProfileAdd_RequestDTO request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result<CandidateProfile_ResponseDTO>> GetCandidateProfileByElectionIdAsync(Guid electionId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result<CandidateProfile_ResponseDTO>> UpdateCandidateProfileByElectionIdAsync(Guid electionId, CandidateProfileUpdate_RequestDTO request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result<string>> UpdateCandidateProfile_Photo_ByElectionIdAsync(Guid electionId, CandidateProfileUploadImage_RequestDTO request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
