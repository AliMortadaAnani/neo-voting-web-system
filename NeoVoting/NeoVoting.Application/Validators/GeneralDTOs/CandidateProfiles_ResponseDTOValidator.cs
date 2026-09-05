using FluentValidation;
using NeoVoting.Application.ResponseDTOs.GeneralDTOs;

namespace NeoVoting.Application.Validators.GeneralDTOs
{
    public class CandidateProfiles_ResponseDTOValidator : AbstractValidator<CandidateProfile_ResponseDTO>
    {
        public CandidateProfiles_ResponseDTOValidator()
        {
            RuleFor(x => x.CandidateProfileId).NotNull();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Gender).NotNull();
            RuleFor(x => x.DateOfBirth).NotNull();
            RuleFor(x => x.NominationReasons).NotEmpty();
            //RuleFor(x => x.VotesCount).NotNull();
        }
    }
}