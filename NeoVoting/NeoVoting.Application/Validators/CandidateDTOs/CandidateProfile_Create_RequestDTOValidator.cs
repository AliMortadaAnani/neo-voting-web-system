using FluentValidation;
using NeoVoting.Application.RequestDTOs.CandidateDTOs;

namespace NeoVoting.Application.Validators.CandidateDTOs
{
    public class CandidateProfile_Create_RequestDTOValidator : AbstractValidator<CandidateProfile_Create_RequestDTO>
    {
        public CandidateProfile_Create_RequestDTOValidator()
        {
            RuleFor(x => x.NominationReasons).NotEmpty().MaximumLength(4000);
        }
    }
}