using NeoVoting.Application.AuthDTOs;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.ErrorHandling;

namespace NeoVoting.Application.ServicesContracts
{
    public interface IAuthServices
    {
        Task<Result<Authentication_ResponseDTO>> LoginAsync(Login_RequestDTO loginDTO, CancellationToken cancellationToken = default);

        Task<Result<bool>> LogoutAsync(CancellationToken cancellationToken = default);

        Task<Result<Registration_ResetPassword_ResponseDTO>> RegisterVoterOrCandidateAsync(Register_ResetPassword_VoterOrCandidate_RequestDTO registrationDTO,RoleTypesEnum role, CancellationToken cancellationToken = default);

        Task<Result<Registration_ResetPassword_ResponseDTO>> ResetVoterOrCandidatePasswordAsync(Register_ResetPassword_VoterOrCandidate_RequestDTO resetPasswordDTO, CancellationToken cancellationToken = default);


        Task<Result<Authentication_ResponseDTO>> RefreshTokenAsync(RefreshToken_RequestDTO refreshTokenRequestDTO, CancellationToken cancellationToken = default);
    }
}