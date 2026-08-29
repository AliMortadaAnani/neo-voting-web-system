using FluentValidation;

namespace NeoVoting.Application.Validators.AdminDTOs
{
    public class SystemAuditLog_ResponseDTO_Validator : AbstractValidator<SystemAuditLog_ResponseDTO>
    {
        public SystemAuditLog_ResponseDTO_Validator()
        {
            // Required fields (non-nullable)
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.TimestampUTC).NotEmpty();
            RuleFor(x => x.ActionType).IsInEnum();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Username).NotEmpty();

            // Optional fields (nullable) - no validation rules needed
            // Details, ElectionName, CandidateProfileId, ElectionId
        }
    }
}