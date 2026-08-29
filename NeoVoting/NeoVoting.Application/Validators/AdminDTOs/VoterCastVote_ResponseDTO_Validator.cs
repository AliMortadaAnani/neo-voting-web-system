using FluentValidation;

namespace NeoVoting.Application.Validators.AdminDTOs
{
    public class VoterCastVote_ResponseDTO_Validator : AbstractValidator<Voter_Cast_Track_Vote_ResponseDTO>
    {
        public VoterCastVote_ResponseDTO_Validator()
        {
            // Required fields (non-nullable)
            RuleFor(x => x.VoteId).NotEmpty();

            // Optional fields (nullable) - no validation rules needed
            // ElectionId, ElectionName, GovernorateId, GovernorateName
        }
    }
}