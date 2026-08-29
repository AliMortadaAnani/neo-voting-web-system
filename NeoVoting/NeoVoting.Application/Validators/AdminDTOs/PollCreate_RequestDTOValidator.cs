using FluentValidation;
using NeoVoting.Application.RequestDTOs.AdminDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Validators.AdminDTOs
{
    public class PollCreate_RequestDTOValidator : AbstractValidator<PollCreate_RequestDTO>
    {
        public PollCreate_RequestDTOValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Question).NotEmpty();
            RuleFor(x => x.StartDate).NotNull();
            RuleFor(x => x.EndDate).NotNull();
            RuleFor(x => x.Answers).NotEmpty();

            RuleFor(x => x)
                .Must(x => x.StartDate < x.EndDate)
                .WithMessage("StartDate must be before EndDate")
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue);

            RuleForEach(x => x.Answers)
                .NotEmpty()
                .When(x => x.Answers != null);
        }
    }
}
