using GovernmentSystem.API.Domain.Enums;

namespace GovernmentSystem.API.Application.ResponseDTOs.CandidateDTOs
{   // response dto dont need to be nullable to avoid .Net 8 nullable warnings when validating since they are only used for output
    //otherwise, we need to make every request dto property nullable to avoid the warnings and rely only on FluentValidation for validation
    public class CandidateResponseDTO
    {
        public int Id { get; set; }
        public string NationalId { get; set; } = string.Empty; 
        public int CitizenId { get; set; }
        public string NominationToken { get; set; } = string.Empty;
        public string HashedData { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public GovernorateIdEnum GovernorateId { get; set; }
        public char Gender { get; set; }

    }
}