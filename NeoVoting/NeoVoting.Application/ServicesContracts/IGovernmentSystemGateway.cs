using NeoVoting.Application.NeoVotingDTOs;
using NeoVoting.Domain.ErrorHandling;

namespace NeoVoting.Application.ServicesContracts
{
    /*
     a gateway is a class or service in your application that acts as a bridge to an external system. It encapsulates all the details of how your app communicates with that external service (such as building HTTP requests, handling responses, and mapping errors), providing your own code with a simple, stable interface.

        In essence:
        A gateway hides the complexity of talking to another system, so the rest of your application interacts with external APIs or services in a clean, reliable, and testable way.
     */

    public interface IGovernmentSystemGateway
    {
        Task<Result<NeoVoting_VoterResponseDTO>> VerifyVoterAsync(NeoVoting_GetVoterRequestDTO request, CancellationToken ct);

        Task<Result<NeoVoting_VoterResponseDTO>> MarkVoterAsRegisteredAsync(NeoVoting_VoterIsRegisteredRequestDTO request, CancellationToken ct);
    }
}