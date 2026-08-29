using FluentValidation;

namespace NeoVoting.Application.Validators.AdminDTOs
{
    public class PublicVoteLog_ResponseDTO_Validator : AbstractValidator<PublicVoteLog_ResponseDTO>
    {
        public PublicVoteLog_ResponseDTO_Validator()
        {
            // Required fields (non-nullable)
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.TimestampUTC).NotEmpty();
            RuleFor(x => x.VoteId).NotEmpty();
            RuleFor(x => x.ElectionId).NotEmpty();
            RuleFor(x => x.GovernorateId).NotEmpty();
            RuleFor(x => x.GovernorateName).NotEmpty();
            RuleFor(x => x.ElectionName).NotEmpty();

            // Optional fields (nullable) - no validation rules needed
            // ErrorMessage
        }
    }
}