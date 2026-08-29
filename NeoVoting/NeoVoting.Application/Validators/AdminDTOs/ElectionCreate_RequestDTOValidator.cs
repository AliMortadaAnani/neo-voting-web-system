using FluentValidation;
using NeoVoting.Application.RequestDTOs.AdminDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            RuleFor(x => x)
                .Must(x => x.NominationStartDate < x.NominationEndDate)
                .WithMessage("NominationStartDate must be before NominationEndDate")
                .When(x => x.NominationStartDate.HasValue && x.NominationEndDate.HasValue);

            RuleFor(x => x)
                .Must(x => x.VotingStartDate < x.VotingEndDate)
                .WithMessage("VotingStartDate must be before VotingEndDate")
                .When(x => x.VotingStartDate.HasValue && x.VotingEndDate.HasValue);

            RuleFor(x => x)
                .Must(x => x.NominationEndDate <= x.VotingStartDate)
                .WithMessage("NominationEndDate must be before or equal to VotingStartDate")
                .When(x => x.NominationEndDate.HasValue && x.VotingStartDate.HasValue);
        }
    }
}
