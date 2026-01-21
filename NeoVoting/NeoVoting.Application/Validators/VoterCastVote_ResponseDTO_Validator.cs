using FluentValidation;
using NeoVoting.Application.ResponseDTOs;

namespace NeoVoting.Application.Validators
{
    public class VoterCastVote_ResponseDTO_Validator : AbstractValidator<VoterCastVote_ResponseDTO>
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
