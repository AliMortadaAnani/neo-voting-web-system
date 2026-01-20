using NeoVoting.Application.AuthDTOs;
using NeoVoting.Domain.ErrorHandling;

namespace NeoVoting.Application.ServicesContracts
{
    public interface IAuthServices
    {
        Task<Result<AuthenticationResponse>> LoginAsync(LoginDTO loginDTO, CancellationToken cancellationToken = default);

        Task<Result<bool>> LogoutAsync(CancellationToken cancellationToken = default);

        Task<Result<Registration_ResetPassword_ResponseDTO>> RegisterVoterAsync(RegisterVoterDTO registrationDTO, CancellationToken cancellationToken = default);

        Task<Result<Registration_ResetPassword_ResponseDTO>> ResetVoterOrCandidatePasswordAsync(ResetVoterOrCandidatePasswordDTO resetPasswordDTO, CancellationToken cancellationToken = default);

        Task<Result<Registration_ResetPassword_ResponseDTO>> RegisterCandidateAsync(RegisterCandidateDTO registrationDTO, CancellationToken cancellationToken = default);

        Task<Result<AuthenticationResponse>> RefreshTokenAsync(RefreshTokenRequestDTO refreshTokenRequestDTO, CancellationToken cancellationToken = default);
    }
}