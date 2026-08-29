namespace NeoVoting.Application.ResponseDTOs.GeneralDTOs
{
    public class CompletedElectionStatistics_ResponseDTO
    {
        // ==================================================================
        // CORE IDENTIFIERS
        // ==================================================================

        public int? ElectionId { get; set; }
        public string? ElectionName { get; set; }

        // If null => Global statistics, otherwise specific governorate
        public int? GovernorateId { get; set; }

        public string? GovernorateName { get; set; }
        public DateOnly ParliamentStartDate { get; set; }
        public DateOnly ParliamentEndDate { get; set; }

        // ==================================================================
        // REGISTERED VOTERS (COUNTS)
        // ==================================================================
        public int? TotalRegisteredVotersCount { get; set; }

        public int? RegisteredMaleVotersCount { get; set; }
        public int? RegisteredFemaleVotersCount { get; set; }
        public int? RegisteredVotersAged18To29Count { get; set; }
        public int? RegisteredVotersAged30To45Count { get; set; }
        public int? RegisteredVotersAged46To64Count { get; set; }
        public int? RegisteredVotersAged65AndOverCount { get; set; }

        // ==================================================================
        // ACTUAL VOTERS (COUNTS)
        // ==================================================================
        public int? TotalActualVotersCount { get; set; }

        public int? ActualMaleVotersCount { get; set; }
        public int? ActualFemaleVotersCount { get; set; }
        public int? ActualVotersAged18To29Count { get; set; }
        public int? ActualVotersAged30To45Count { get; set; }
        public int? ActualVotersAged46To64Count { get; set; }
        public int? ActualVotersAged65AndOverCount { get; set; }

        // ==================================================================
        // NOMINATED CANDIDATES (COUNTS)
        // ==================================================================
        public int? TotalNominatedCandidatesCount { get; set; }

        public int? NominatedMaleCandidatesCount { get; set; }
        public int? NominatedFemaleCandidatesCount { get; set; }
        public int? NominatedCandidatesAged18To29Count { get; set; }
        public int? NominatedCandidatesAged30To45Count { get; set; }
        public int? NominatedCandidatesAged46To64Count { get; set; }
        public int? NominatedCandidatesAged65AndOverCount { get; set; }

        // ==================================================================
        // WINNING CANDIDATES (COUNTS)
        // ==================================================================
        public int? TotalWinningCandidatesCount { get; set; } // Always 5 (gov) or 25 (global)

        public int? WinningMaleCandidatesCount { get; set; }
        public int? WinningFemaleCandidatesCount { get; set; }
        public int? WinningCandidatesAged18To29Count { get; set; }
        public int? WinningCandidatesAged30To45Count { get; set; }
        public int? WinningCandidatesAged46To64Count { get; set; }
        public int? WinningCandidatesAged65AndOverCount { get; set; }

        // ==================================================================
        // PERCENTAGES: PERCENTAGE OF REGISTERED VOTERS WHO ACTUALLY VOTED
        // ==================================================================
        public double? PercentageOfTotalRegisteredVotersWhoAreActualVoters { get; set; }

        public double? PercentageOfRegisteredMaleVotersWhoAreActualMaleVoters { get; set; }
        public double? PercentageOfRegisteredFemaleVotersWhoAreActualFemaleVoters { get; set; }

        public double? PercentageOfRegisteredVotersAged18To29WhoAreActualVoters { get; set; }
        public double? PercentageOfRegisteredVotersAged30To45WhoAreActualVoters { get; set; }
        public double? PercentageOfRegisteredVotersAged46To64WhoAreActualVoters { get; set; }
        public double? PercentageOfRegisteredVotersAged65AndOverWhoAreActualVoters { get; set; }

        // ==================================================================
        // PERCENTAGES: SHARE OF TOTAL ACTUAL VOTERS
        // ==================================================================
        public double? PercentageOfTotalActualVotersWhoAreMale { get; set; }

        public double? PercentageOfTotalActualVotersWhoAreFemale { get; set; }

        public double? PercentageOfTotalActualVotersWhoAreAged18To29 { get; set; }
        public double? PercentageOfTotalActualVotersWhoAreAged30To45 { get; set; }
        public double? PercentageOfTotalActualVotersWhoAreAged46To64 { get; set; }
        public double? PercentageOfTotalActualVotersWhoAreAged65AndOver { get; set; }

        // ==================================================================
        // PERCENTAGES: SHARE OF TOTAL NOMINATED CANDIDATES
        // ==================================================================
        public double? PercentageOfTotalNominatedCandidatesWhoAreMale { get; set; }

        public double? PercentageOfTotalNominatedCandidatesWhoAreFemale { get; set; }

        public double? PercentageOfTotalNominatedCandidatesWhoAreAged18To29 { get; set; }
        public double? PercentageOfTotalNominatedCandidatesWhoAreAged30To45 { get; set; }
        public double? PercentageOfTotalNominatedCandidatesWhoAreAged46To64 { get; set; }
        public double? PercentageOfTotalNominatedCandidatesWhoAreAged65AndOver { get; set; }

        // ==================================================================
        // PERCENTAGES: SHARE OF TOTAL WINNING CANDIDATES
        // ==================================================================
        public double? PercentageOfTotalWinningCandidatesWhoAreMale { get; set; }

        public double? PercentageOfTotalWinningCandidatesWhoAreFemale { get; set; }

        public double? PercentageOfTotalWinningCandidatesWhoAreAged18To29 { get; set; }
        public double? PercentageOfTotalWinningCandidatesWhoAreAged30To45 { get; set; }
        public double? PercentageOfTotalWinningCandidatesWhoAreAged46To64 { get; set; }
        public double? PercentageOfTotalWinningCandidatesWhoAreAged65AndOver { get; set; }

        // ==================================================================
        // COMPARISONS & DIFFERENCES (EXPLICIT ENGLISH METRICS)
        // ==================================================================

        public double? DifferenceBetweenMaleAndFemaleActualVoterSharePercentage { get; set; }
        public double? DifferenceBetweenMaleAndFemaleNominatedCandidateSharePercentage { get; set; }
        public double? DifferenceBetweenMaleAndFemaleWinningCandidateSharePercentage { get; set; }

        // Representation Comparison (Winning Candidate Share minus Actual Voter Share)
        public double? DifferenceBetweenWinningMaleShareAndActualMaleVoterShare { get; set; }

        public double? DifferenceBetweenWinningShareAndActualVoterShareForAged18To29 { get; set; }
        public double? DifferenceBetweenWinningShareAndActualVoterShareForAged30To45 { get; set; }
        public double? DifferenceBetweenWinningShareAndActualVoterShareForAged46To64 { get; set; }
        public double? DifferenceBetweenWinningShareAndActualVoterShareForAged65AndOver { get; set; }
    }
}