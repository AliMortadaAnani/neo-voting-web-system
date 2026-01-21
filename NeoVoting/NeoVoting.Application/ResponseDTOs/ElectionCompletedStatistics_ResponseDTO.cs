using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.ResponseDTOs
{
    public class ElectionCompletedStatistics_ResponseDTO
    {
        // Voter statistics
        public int? RegisteredVotersCount { get; set; }
        public int? ActualVotersCount { get; set; }
        public double? ParticipationPercentage { get; set; } // (ActualVotersCount / RegisteredVotersCount) * 100

        // Gender breakdown
        public int? RegisteredMalesCount { get; set; }
        public int? RegisteredFemalesCount { get; set; }
        public int? VotedMalesCount { get; set; }
        public int? VotedFemalesCount { get; set; }

        // Gender participation percentages
        public double? MaleVotedOutOfRegisteredPercentage { get; set; } // (VotedMalesCount / RegisteredMalesCount) * 100
        public double? FemaleVotedOutOfRegisteredPercentage { get; set; } // (VotedFemalesCount / RegisteredFemalesCount) * 100
        public double? MaleVotedOutOfTotalVotedPercentage { get; set; } // (VotedMalesCount / ActualVotersCount) * 100
        public double? FemaleVotedOutOfTotalVotedPercentage { get; set; } // (VotedFemalesCount / ActualVotersCount) * 100

        // Age group breakdown (18-29, 30-45, 46-64, 65+)
        public int? RegisteredAge18To29Count { get; set; }
        public int? RegisteredAge30To45Count { get; set; }
        public int? RegisteredAge46To64Count { get; set; }
        public int? RegisteredAge65AndOverCount { get; set; }
        public int? VotedAge18To29Count { get; set; }
        public int? VotedAge30To45Count { get; set; }
        public int? VotedAge46To64Count { get; set; }
        public int? VotedAge65AndOverCount { get; set; }

        // Age group participation percentages
        public double? Age18To29VotedOutOfRegisteredPercentage { get; set; } // (VotedAge18To29Count / RegisteredAge18To29Count) * 100
        public double? Age30To45VotedOutOfRegisteredPercentage { get; set; }
        public double? Age46To64VotedOutOfRegisteredPercentage { get; set; }
        public double? Age65AndOverVotedOutOfRegisteredPercentage { get; set; }

        public double? Age18To29VotedOutOfTotalVotedPercentage { get; set; } // (VotedAge18To29Count / ActualVotersCount) * 100
        public double? Age30To45VotedOutOfTotalVotedPercentage { get; set; }
        public double? Age46To64VotedOutOfTotalVotedPercentage { get; set; }
        public double? Age65AndOverVotedOutOfTotalVotedPercentage { get; set; }

        // Candidates and winners
        public int? CandidatesNominatedCount { get; set; }
        
        public List<string>? WinningCandidates { get; set; }

        // Election dates
        public DateTime? NominationStartDate { get; set; }
        public DateTime? NominationEndDate { get; set; }
        public DateTime? VotingStartDate { get; set; }
        public DateTime? VotingEndDate { get; set; }

        // Parliamentary term dates
        public DateTime? ParliamentaryTermStartDate { get; set; }
        public DateTime? ParliamentaryTermEndDate { get; set; }
    }


}
