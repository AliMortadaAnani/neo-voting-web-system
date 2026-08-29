using FluentValidation;
using NeoVoting.Application.ResponseDTOs.GeneralDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Validators.GeneralDTOs
{
    public class CompletedElectionStatistics_ResponseDTOValidator : AbstractValidator<CompletedElectionStatistics_ResponseDTO>
    {
        public CompletedElectionStatistics_ResponseDTOValidator()
        {
            RuleFor(x => x.ElectionId).NotNull();
            RuleFor(x => x.ElectionName).NotEmpty();
            RuleFor(x => x.ParliamentStartDate).NotNull();
            RuleFor(x => x.ParliamentEndDate).NotNull();
            RuleFor(x => x.TotalRegisteredVotersCount).NotNull();
            RuleFor(x => x.TotalActualVotersCount).NotNull();
            RuleFor(x => x.TotalNominatedCandidatesCount).NotNull();
            RuleFor(x => x.TotalWinningCandidatesCount).NotNull();
            RuleFor(x => x.PercentageOfTotalRegisteredVotersWhoAreActualVoters).NotNull();

            // All percentage fields should not be null
            RuleFor(x => x.PercentageOfTotalActualVotersWhoAreMale).NotNull();
            RuleFor(x => x.PercentageOfTotalActualVotersWhoAreFemale).NotNull();
            RuleFor(x => x.PercentageOfTotalNominatedCandidatesWhoAreMale).NotNull();
            RuleFor(x => x.PercentageOfTotalNominatedCandidatesWhoAreFemale).NotNull();
            RuleFor(x => x.PercentageOfTotalWinningCandidatesWhoAreMale).NotNull();
            RuleFor(x => x.PercentageOfTotalWinningCandidatesWhoAreFemale).NotNull();

            // Ensure percentages are between 0 and 100
            RuleFor(x => x.PercentageOfTotalRegisteredVotersWhoAreActualVoters)
                .InclusiveBetween(0, 100)
                .When(x => x.PercentageOfTotalRegisteredVotersWhoAreActualVoters.HasValue);

            RuleFor(x => x.PercentageOfTotalActualVotersWhoAreMale)
                .InclusiveBetween(0, 100)
                .When(x => x.PercentageOfTotalActualVotersWhoAreMale.HasValue);

            RuleFor(x => x.PercentageOfTotalActualVotersWhoAreFemale)
                .InclusiveBetween(0, 100)
                .When(x => x.PercentageOfTotalActualVotersWhoAreFemale.HasValue);

            RuleFor(x => x.PercentageOfTotalNominatedCandidatesWhoAreMale)
                .InclusiveBetween(0, 100)
                .When(x => x.PercentageOfTotalNominatedCandidatesWhoAreMale.HasValue);

            RuleFor(x => x.PercentageOfTotalNominatedCandidatesWhoAreFemale)
                .InclusiveBetween(0, 100)
                .When(x => x.PercentageOfTotalNominatedCandidatesWhoAreFemale.HasValue);

            RuleFor(x => x.PercentageOfTotalWinningCandidatesWhoAreMale)
                .InclusiveBetween(0, 100)
                .When(x => x.PercentageOfTotalWinningCandidatesWhoAreMale.HasValue);

            RuleFor(x => x.PercentageOfTotalWinningCandidatesWhoAreFemale)
                .InclusiveBetween(0, 100)
                .When(x => x.PercentageOfTotalWinningCandidatesWhoAreFemale.HasValue);
        }
    }
}
