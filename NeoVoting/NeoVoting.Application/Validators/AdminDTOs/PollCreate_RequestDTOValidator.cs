using FluentValidation;
using NeoVoting.Application.RequestDTOs.AdminDTOs;

namespace NeoVoting.Application.Validators.AdminDTOs
{
    public class PollCreate_RequestDTOValidator : AbstractValidator<PollCreate_RequestDTO>
    {
        public PollCreate_RequestDTOValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Question).NotEmpty().MaximumLength(4000);
            RuleFor(x => x.StartDate).NotNull();
            RuleFor(x => x.EndDate).NotNull();
            RuleFor(x => x.Answers).NotEmpty();

            RuleForEach(x => x.Answers)
            .NotEmpty()
            .MaximumLength(4000)
            .When(x => x.Answers != null);
        }
    }
}