using NeoVoting.Domain.Enums;
using System;
using System.Text;

namespace NeoVoting.Domain.Entities
{
    public class ElectionStatistics
    {
        // E : Election  P : Poll  EP : Election and Poll Statistics
        // since Elections reveals who the people want as Representatives then statistics are highly relevant
        // to the contrary, since Polls are the way to show people Honest opinions without revealing
        // their identity, and to avoid any kind of judgement on certain people based on 
        //Age,Gender,Governorate, etc. then statistics are not needed in this case

        public int Id { get; set; } //ep
 


        public int ElectionId { get; set; } //e
        public GovernorateIdEnum? Governorate { get; set; } //e 
        // if Governorate is null then it means the statistics are for the whole country, otherwise they are for a specific governorate

        public Election Election { get; set; }

        // ==================================================================
        // 1. RAW COUNTS (Nullable)
        // ==================================================================

        // General
        public int? CandidatesNominatedCount { get; set; } //e
        public int? RegisteredVotersCount { get; set; } //ep // calculated at the end of the election/poll
        public int? ActualVotersCount { get; set; } //ep // from Vote tables

        // Gender - Voters
        public int? RegisteredMalesCount { get; set; } //e
        public int? RegisteredFemalesCount { get; set; } //e
        public int? VotedMalesCount { get; set; } //e
        public int? VotedFemalesCount { get; set; } //e

        // Gender - Candidates
        public int? NominatedMaleCandidatesCount { get; set; } //e
        public int? NominatedFemaleCandidatesCount { get; set; } //e
        public int? WinnerMaleCandidatesCount { get; set; } //e
        public int? WinnerFemaleCandidatesCount { get; set; } //e

        // Age Groups (Registered Voters) //e
        public int? RegisteredAge18To29Count { get; set; }
        public int? RegisteredAge30To45Count { get; set; }
        public int? RegisteredAge46To64Count { get; set; }
        public int? RegisteredAge65AndOverCount { get; set; }

        // Age Groups (Voted Voters) //e
        public int? VotedAge18To29Count { get; set; }
        public int? VotedAge30To45Count { get; set; }
        public int? VotedAge46To64Count { get; set; }
        public int? VotedAge65AndOverCount { get; set; }

        // Age Groups - Nominated Candidates //e
        public int? NominatedCandidatesAge18To29Count { get; set; }
        public int? NominatedCandidatesAge30To45Count { get; set; }
        public int? NominatedCandidatesAge46To64Count { get; set; }
        public int? NominatedCandidatesAge65AndOverCount { get; set; }

        // Age Groups - Winner Candidates //e
        public int? WinnerCandidatesAge18To29Count { get; set; }
        public int? WinnerCandidatesAge30To45Count { get; set; }
        public int? WinnerCandidatesAge46To64Count { get; set; }
        public int? WinnerCandidatesAge65AndOverCount { get; set; }

        // ==================================================================
        // 2. PERCENTAGES (Nullable)
        // ==================================================================

        public double? ParticipationPercentage { get; set; } //ep

        // Gender Participation - Voters //e
        public double? MaleVotedOutOfRegisteredPercentage { get; set; }
        public double? FemaleVotedOutOfRegisteredPercentage { get; set; }
        public double? MaleVotedOutOfTotalVotedPercentage { get; set; }
        public double? FemaleVotedOutOfTotalVotedPercentage { get; set; }

        // Gender Participation - Candidates //e
        public double? FemaleNominatedOutOfTotalNominatedPercentage { get; set; }
        public double? FemaleWinnersOutOfTotalWinnersPercentage { get; set; }
        public double? MaleNominatedOutOfTotalNominatedPercentage { get; set; }
        public double? MaleWinnersOutOfTotalWinnersPercentage { get; set; }

        // Age Participation (Out of Registered) - Voters //e
        public double? Age18To29VotedOutOfRegisteredPercentage { get; set; }
        public double? Age30To45VotedOutOfRegisteredPercentage { get; set; }
        public double? Age46To64VotedOutOfRegisteredPercentage { get; set; }
        public double? Age65AndOverVotedOutOfRegisteredPercentage { get; set; }

        // Age Participation (Out of Total Voted) - Voters //e
        public double? Age18To29VotedOutOfTotalVotedPercentage { get; set; }
        public double? Age30To45VotedOutOfTotalVotedPercentage { get; set; }
        public double? Age46To64VotedOutOfTotalVotedPercentage { get; set; }
        public double? Age65AndOverVotedOutOfTotalVotedPercentage { get; set; }

        // Age Participation - Candidates (Out of Total Nominated) //e
        public double? Age18To29NominatedOutOfTotalNominatedPercentage { get; set; }
        public double? Age30To45NominatedOutOfTotalNominatedPercentage { get; set; }
        public double? Age46To64NominatedOutOfTotalNominatedPercentage { get; set; }
        public double? Age65AndOverNominatedOutOfTotalNominatedPercentage { get; set; }

        // Age Participation - Winners (Out of Total Winners) //e
        public double? Age18To29WinnersOutOfTotalWinnersPercentage { get; set; }
        public double? Age30To45WinnersOutOfTotalWinnersPercentage { get; set; }
        public double? Age46To64WinnersOutOfTotalWinnersPercentage { get; set; }
        public double? Age65AndOverWinnersOutOfTotalWinnersPercentage { get; set; }

        

        public ElectionStatistics()
        {
            Election = null!;
        }
    }
}