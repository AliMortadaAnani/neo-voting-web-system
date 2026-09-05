using FluentValidation;
using NeoVoting.Application.RequestDTOs.AdminDTOs;

namespace NeoVoting.Application.Validators.AdminDTOs
{
    public class ElectionCreate_RequestDTOValidator : AbstractValidator<ElectionCreate_RequestDTO>
    {
        public ElectionCreate_RequestDTOValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.NominationStartDate).NotNull();
            RuleFor(x => x.NominationEndDate).NotNull();
            RuleFor(x => x.VotingStartDate).NotNull();
            RuleFor(x => x.VotingEndDate).NotNull();
        }
    }
}