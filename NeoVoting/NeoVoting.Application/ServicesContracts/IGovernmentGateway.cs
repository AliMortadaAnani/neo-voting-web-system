using NeoVoting.Application.NeoVotingDTOs;
using NeoVoting.Domain.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.ServicesContracts
{
    public interface IGovernmentGateway
    {
        Task<Result<NeoVoting_GetVoterRequestDTO>> VerifyVoterAsync(NeoVoting_GetVoterRequestDTO requestDTO, CancellationToken ct);
        Task<Result<NeoVoting_GetVoterRequestDTO>> MarkVoterAsRegisteredAsync(NeoVoting_VoterIsRegisteredRequestDTO requestDTO, CancellationToken ct);
    }
}
