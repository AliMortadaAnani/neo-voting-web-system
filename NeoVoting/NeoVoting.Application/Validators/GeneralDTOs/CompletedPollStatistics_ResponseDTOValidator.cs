using FluentValidation;
using NeoVoting.Application.ResponseDTOs.GeneralDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Validators.GeneralDTOs
{
    public class CompletedPollStatistics_ResponseDTOValidator : AbstractValidator<CompletedPollStatistics_ResponseDTO>
    {
        public CompletedPollStatistics_ResponseDTOValidator()
        {
            RuleFor(x => x.PollId).NotNull();
            RuleFor(x => x.PollName).NotEmpty();
            RuleFor(x => x.Question).NotEmpty();
            RuleFor(x => x.StartDate).NotNull();
            RuleFor(x => x.EndDate).NotNull();
            RuleFor(x => x.Answers).NotNull();
            RuleFor(x => x.RegisteredVotersCount).NotNull();
            RuleFor(x => x.ActualVotersCount).NotNull();
            RuleFor(x => x.ParticipationPercentage).NotNull();
            RuleFor(x => x.WinnerAnswer).NotEmpty();
            RuleFor(x => x.WinnerVotesCount).NotNull();
            RuleFor(x => x.WinnerVotesPercentage).NotNull();

            RuleFor(x => x.ParticipationPercentage)
                .InclusiveBetween(0, 100)
                .When(x => x.ParticipationPercentage.HasValue);

            RuleFor(x => x.WinnerVotesPercentage)
                .InclusiveBetween(0, 100)
                .When(x => x.WinnerVotesPercentage.HasValue);
        }
    }

    public class CompletedPollAnswerListDTOValidator : AbstractValidator<CompletedPollAnswerListDTO>
    {
        public CompletedPollAnswerListDTOValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.Answer).NotEmpty();
            RuleFor(x => x.VotesCount).NotNull();
            RuleFor(x => x.VotesPercentage).NotNull();

            RuleFor(x => x.VotesPercentage)
                .InclusiveBetween(0, 100)
                .When(x => x.VotesPercentage.HasValue);
        }
    }
}
