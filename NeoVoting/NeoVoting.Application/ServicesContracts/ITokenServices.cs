using NeoVoting.Application.ResponseDTOs.AuthDTOs;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.ResultErrorDomain;
using System.Security.Claims;

namespace NeoVoting.Application.ServicesContracts
{
    public interface ITokenServices
    {
        Task<Authentication_ResponseDTO> CreateAdminTokensAsync(ApplicationUser user);

        Task<Authentication_ResponseDTO> CreateCandidateTokensAsync(ApplicationUser user, Candidate candidate);

        Task<Authentication_ResponseDTO> CreateVoterTokensAsync(ApplicationUser user, Voter voter);

        Result<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token);
    }
}