using NeoVoting.Application.AuthDTOs;
using NeoVoting.Domain.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.ServicesContracts
{
    public interface IAuthServices 
    {
        Task<Result<AuthenticationResponse>> LoginAsync(LoginDTO loginDTO,CancellationToken cancellationToken = default);

        Task<Result<bool>> LogoutAsync(LogoutDTO logoutDTO,CancellationToken cancellationToken = default);

        Task<Result<Registration_ResetPassword_ResponseDTO>> RegisterVoterAsync(RegisterVoterDTO registrationDTO, CancellationToken cancellationToken = default);

        Task<Result<Registration_ResetPassword_ResponseDTO>> ResetVoterPasswordAsync(ResetVoterPasswordDTO resetPasswordDTO, CancellationToken cancellationToken = default);


        Task<Result<Registration_ResetPassword_ResponseDTO>> RegisterCandidateAsync(RegisterCandidateDTO registrationDTO, CancellationToken cancellationToken = default);

        Task<Result<Registration_ResetPassword_ResponseDTO>> ResetCandidatePasswordAsync(ResetCandidatePasswordDTO resetPasswordDTO, CancellationToken cancellationToken = default);


        Task<Result<AuthenticationResponse>> RefreshTokenAsync(RefreshTokenRequestDTO refreshTokenRequestDTO, CancellationToken cancellationToken = default);




    }
}
