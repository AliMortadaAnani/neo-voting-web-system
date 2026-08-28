using NeoVoting.Domain.Enums;
using NeoVoting.Domain.ResultErrorDomain;

namespace NeoVoting.Application.ServicesContracts
{
    /*
     a gateway is a class or service in your application that acts as a bridge to an external system. It encapsulates all the details of how your app communicates with that external service (such as building HTTP requests, handling responses, and mapping errors), providing your own code with a simple, stable interface.

        In essence:
        A gateway hides the complexity of talking to another system, so the rest of your application interacts with external APIs or services in a clean, reliable, and testable way.
     */

    public interface IGovernmentSystemGateway
    {
        Task<Result<VoterVerificationResponseDTO>> VerifyVoterAsync(GetVoterVerificationRequestDTO request);

        Task<Result<CandidateVerificationResponseDTO>> VerifyCandidateAsync(GetCandidateVerificationRequestDTO request);
    }

    public class GetCandidateVerificationRequestDTO
    {
        public string NationalId { get; set; } = string.Empty;

        public string NominationToken { get; set; } = string.Empty;
    }

    public class GetVoterVerificationRequestDTO
    {
        public string NationalId { get; set; } = string.Empty;

        public string VotingToken { get; set; } = string.Empty;
    }

    public class CandidateVerificationResponseDTO
    {
        public string HashedData { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public GovernorateIdEnum Governorate { get; set; }
        public char Gender { get; set; }
    }

    public class VoterVerificationResponseDTO
    {
        public string HashedData { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public GovernorateIdEnum Governorate { get; set; }
        public char Gender { get; set; }
    }
}