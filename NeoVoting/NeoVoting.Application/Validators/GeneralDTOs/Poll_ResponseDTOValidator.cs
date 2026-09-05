using FluentValidation;
using NeoVoting.Application.ResponseDTOs.GeneralDTOs;

namespace NeoVoting.Application.Validators.GeneralDTOs
{
    public class Poll_ResponseDTOValidator : AbstractValidator<Poll_ResponseDTO>
    {
        public Poll_ResponseDTOValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Question).NotEmpty();
            RuleFor(x => x.StartDate).NotNull();
            RuleFor(x => x.EndDate).NotNull();
            RuleFor(x => x.Answers).NotNull();
            RuleFor(x => x.Status).NotNull();
        }
    }

    public class PollAnswerListDTOValidator : AbstractValidator<PollAnswerDTO>
    {
        public PollAnswerListDTOValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.Answer).NotEmpty();
            // RuleFor(x => x.VotesCount).NotNull();
        }
    }
}