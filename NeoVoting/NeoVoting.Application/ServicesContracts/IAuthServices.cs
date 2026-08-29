using NeoVoting.Domain.Enums;
using NeoVoting.Domain.ResultErrorDomain;

namespace NeoVoting.Application.ServicesContracts
{
    public interface IAuthServices
    {
        Task<Result<Authentication_ResponseDTO>> LoginAsync(Login_RequestDTO loginDTO);

        Task<Result<bool>> LogoutAsync();

        Task<Result<RegisterVoterOrCandidate_ResponseDTO>> RegisterVoterOrCandidateAsync(Register_ResetPassword_VoterOrCandidate_RequestDTO registrationDTO, RoleTypesEnum role);

        Task<Result<RegisterVoterOrCandidate_ResponseDTO>> ResetVoterOrCandidatePasswordAsync(Register_ResetPassword_VoterOrCandidate_RequestDTO resetPasswordDTO);

        Task<Result<Authentication_ResponseDTO>> RefreshTokenAsync(RefreshToken_RequestDTO refreshTokenRequestDTO);
    }
}