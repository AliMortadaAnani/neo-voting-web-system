using NeoVoting.Domain.Enums;

namespace NeoVoting.Domain.Entities
{
    public class ElectionStatistics
    {
        // ==================================================================
        // CORE IDENTIFIERS
        // ==================================================================
        public int Id { get; set; }

        public int ElectionId { get; set; }
        public Election Election { get; set; }

        // If null => Global statistics, otherwise specific governorate
        public GovernorateIdEnum? Governorate { get; set; }

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

        // ==================================================================
        // CONSTRUCTORS
        // ==================================================================

        public ElectionStatistics()
        {
            Election = null!;
        }

        public ElectionStatistics(
            int electionId,
            GovernorateIdEnum? governorate = null,
            DateOnly? parliamentStartDate = null,
            DateOnly? parliamentEndDate = null)
        {
            ElectionId = electionId;
            Governorate = governorate;
            ParliamentStartDate = parliamentStartDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
            ParliamentEndDate = parliamentEndDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddYears(4));
            Election = null!;
        }

        // ==================================================================
        // DATA POPULATION METHODS
        // ==================================================================

        public void PopulateVoterData(
            int? totalRegistered,
            int? registeredMales,
            int? registeredFemales,
            int? registeredAge18To29,
            int? registeredAge30To45,
            int? registeredAge46To64,
            int? registeredAge65AndOver,
            int? totalActualVoters,
            int? actualVotersMales,
            int? actualVotersFemales,
            int? actualVotersAge18To29,
            int? actualVotersAge30To45,
            int? actualVotersAge46To64,
            int? actualVotersAge65AndOver)
        {
            TotalRegisteredVotersCount = totalRegistered;
            RegisteredMaleVotersCount = registeredMales;
            RegisteredFemaleVotersCount = registeredFemales;
            RegisteredVotersAged18To29Count = registeredAge18To29;
            RegisteredVotersAged30To45Count = registeredAge30To45;
            RegisteredVotersAged46To64Count = registeredAge46To64;
            RegisteredVotersAged65AndOverCount = registeredAge65AndOver;

            TotalActualVotersCount = totalActualVoters;
            ActualMaleVotersCount = actualVotersMales;
            ActualFemaleVotersCount = actualVotersFemales;
            ActualVotersAged18To29Count = actualVotersAge18To29;
            ActualVotersAged30To45Count = actualVotersAge30To45;
            ActualVotersAged46To64Count = actualVotersAge46To64;
            ActualVotersAged65AndOverCount = actualVotersAge65AndOver;

            // Percentage of Registered Voters Who Actually Voted
            PercentageOfTotalRegisteredVotersWhoAreActualVoters = CalculatePercentage(totalActualVoters, totalRegistered);
            PercentageOfRegisteredMaleVotersWhoAreActualMaleVoters = CalculatePercentage(actualVotersMales, registeredMales);
            PercentageOfRegisteredFemaleVotersWhoAreActualFemaleVoters = CalculatePercentage(actualVotersFemales, registeredFemales);

            PercentageOfRegisteredVotersAged18To29WhoAreActualVoters = CalculatePercentage(actualVotersAge18To29, registeredAge18To29);
            PercentageOfRegisteredVotersAged30To45WhoAreActualVoters = CalculatePercentage(actualVotersAge30To45, registeredAge30To45);
            PercentageOfRegisteredVotersAged46To64WhoAreActualVoters = CalculatePercentage(actualVotersAge46To64, registeredAge46To64);
            PercentageOfRegisteredVotersAged65AndOverWhoAreActualVoters = CalculatePercentage(actualVotersAge65AndOver, registeredAge65AndOver);

            // Share of Total Actual Voters
            PercentageOfTotalActualVotersWhoAreMale = CalculatePercentage(actualVotersMales, totalActualVoters);
            PercentageOfTotalActualVotersWhoAreFemale = CalculatePercentage(actualVotersFemales, totalActualVoters);

            PercentageOfTotalActualVotersWhoAreAged18To29 = CalculatePercentage(actualVotersAge18To29, totalActualVoters);
            PercentageOfTotalActualVotersWhoAreAged30To45 = CalculatePercentage(actualVotersAge30To45, totalActualVoters);
            PercentageOfTotalActualVotersWhoAreAged46To64 = CalculatePercentage(actualVotersAge46To64, totalActualVoters);
            PercentageOfTotalActualVotersWhoAreAged65AndOver = CalculatePercentage(actualVotersAge65AndOver, totalActualVoters);

            DifferenceBetweenMaleAndFemaleActualVoterSharePercentage = CalculateDifference(PercentageOfTotalActualVotersWhoAreMale, PercentageOfTotalActualVotersWhoAreFemale);
        }

        public void PopulateCandidatesData(
            int? totalNominated,
            int? nominatedMales,
            int? nominatedFemales,
            int? nominatedAge18To29,
            int? nominatedAge30To45,
            int? nominatedAge46To64,
            int? nominatedAge65AndOver)
        {
            TotalNominatedCandidatesCount = totalNominated;
            NominatedMaleCandidatesCount = nominatedMales;
            NominatedFemaleCandidatesCount = nominatedFemales;
            NominatedCandidatesAged18To29Count = nominatedAge18To29;
            NominatedCandidatesAged30To45Count = nominatedAge30To45;
            NominatedCandidatesAged46To64Count = nominatedAge46To64;
            NominatedCandidatesAged65AndOverCount = nominatedAge65AndOver;

            PercentageOfTotalNominatedCandidatesWhoAreMale = CalculatePercentage(nominatedMales, totalNominated);
            PercentageOfTotalNominatedCandidatesWhoAreFemale = CalculatePercentage(nominatedFemales, totalNominated);

            PercentageOfTotalNominatedCandidatesWhoAreAged18To29 = CalculatePercentage(nominatedAge18To29, totalNominated);
            PercentageOfTotalNominatedCandidatesWhoAreAged30To45 = CalculatePercentage(nominatedAge30To45, totalNominated);
            PercentageOfTotalNominatedCandidatesWhoAreAged46To64 = CalculatePercentage(nominatedAge46To64, totalNominated);
            PercentageOfTotalNominatedCandidatesWhoAreAged65AndOver = CalculatePercentage(nominatedAge65AndOver, totalNominated);

            DifferenceBetweenMaleAndFemaleNominatedCandidateSharePercentage = CalculateDifference(PercentageOfTotalNominatedCandidatesWhoAreMale, PercentageOfTotalNominatedCandidatesWhoAreFemale);
        }

        public void PopulateWinnersData(List<ElectionWinner> winners)
        {
            if (winners == null || !winners.Any())
            {
                TotalWinningCandidatesCount = 0;
                return;
            }

            TotalWinningCandidatesCount = winners.Count;

            var today = DateOnly.FromDateTime(DateTime.Today);

            WinningMaleCandidatesCount = winners.Count(c => c.CandidateProfile.Candidate.Gender == 'M' || c.CandidateProfile.Candidate.Gender == 'm');
            WinningFemaleCandidatesCount = winners.Count(c => c.CandidateProfile.Candidate.Gender == 'F' || c.CandidateProfile.Candidate.Gender == 'f');

            WinningCandidatesAged18To29Count = winners.Count(c =>
            {
                var age = CalculateAge(c.CandidateProfile.Candidate.DateOfBirth, today);
                return age >= 18 && age <= 29;
            });

            WinningCandidatesAged30To45Count = winners.Count(c =>
            {
                var age = CalculateAge(c.CandidateProfile.Candidate.DateOfBirth, today);
                return age >= 30 && age <= 45;
            });

            WinningCandidatesAged46To64Count = winners.Count(c =>
            {
                var age = CalculateAge(c.CandidateProfile.Candidate.DateOfBirth, today);
                return age >= 46 && age <= 64;
            });

            WinningCandidatesAged65AndOverCount = winners.Count(c =>
            {
                var age = CalculateAge(c.CandidateProfile.Candidate.DateOfBirth, today);
                return age >= 65;
            });

            PercentageOfTotalWinningCandidatesWhoAreMale = CalculatePercentage(WinningMaleCandidatesCount, TotalWinningCandidatesCount);
            PercentageOfTotalWinningCandidatesWhoAreFemale = CalculatePercentage(WinningFemaleCandidatesCount, TotalWinningCandidatesCount);

            PercentageOfTotalWinningCandidatesWhoAreAged18To29 = CalculatePercentage(WinningCandidatesAged18To29Count, TotalWinningCandidatesCount);
            PercentageOfTotalWinningCandidatesWhoAreAged30To45 = CalculatePercentage(WinningCandidatesAged30To45Count, TotalWinningCandidatesCount);
            PercentageOfTotalWinningCandidatesWhoAreAged46To64 = CalculatePercentage(WinningCandidatesAged46To64Count, TotalWinningCandidatesCount);
            PercentageOfTotalWinningCandidatesWhoAreAged65AndOver = CalculatePercentage(WinningCandidatesAged65AndOverCount, TotalWinningCandidatesCount);

            DifferenceBetweenMaleAndFemaleWinningCandidateSharePercentage = CalculateDifference(PercentageOfTotalWinningCandidatesWhoAreMale, PercentageOfTotalWinningCandidatesWhoAreFemale);

            DifferenceBetweenWinningMaleShareAndActualMaleVoterShare = CalculateDifference(PercentageOfTotalWinningCandidatesWhoAreMale, PercentageOfTotalActualVotersWhoAreMale);

            DifferenceBetweenWinningShareAndActualVoterShareForAged18To29 = CalculateDifference(PercentageOfTotalWinningCandidatesWhoAreAged18To29, PercentageOfTotalActualVotersWhoAreAged18To29);
            DifferenceBetweenWinningShareAndActualVoterShareForAged30To45 = CalculateDifference(PercentageOfTotalWinningCandidatesWhoAreAged30To45, PercentageOfTotalActualVotersWhoAreAged30To45);
            DifferenceBetweenWinningShareAndActualVoterShareForAged46To64 = CalculateDifference(PercentageOfTotalWinningCandidatesWhoAreAged46To64, PercentageOfTotalActualVotersWhoAreAged46To64);
            DifferenceBetweenWinningShareAndActualVoterShareForAged65AndOver = CalculateDifference(PercentageOfTotalWinningCandidatesWhoAreAged65AndOver, PercentageOfTotalActualVotersWhoAreAged65AndOver);
        }

        // ==================================================================
        // HELPER METHODS
        // ==================================================================

        private static double? CalculatePercentage(int? part, int? whole)
        {
            if (!part.HasValue || !whole.HasValue || whole.Value == 0)
                return null;

            return Math.Round((part.Value / (double)whole.Value) * 100, 2);
        }

        private static double? CalculateDifference(double? value1, double? value2)
        {
            if (!value1.HasValue || !value2.HasValue)
                return null;

            return Math.Round(value1.Value - value2.Value, 2);
        }

        public override string ToString()
        {
            var scope = Governorate.HasValue ? $"Governorate: {Governorate}" : "Global";
            return $"{scope} - Election {ElectionId}: {TotalRegisteredVotersCount} registered voters, {TotalActualVotersCount} actual voters, {TotalNominatedCandidatesCount} nominated candidates, {TotalWinningCandidatesCount} winning candidates";
        }

        private int CalculateAge(DateOnly dateOfBirth, DateOnly currentDate)
        {
            var age = currentDate.Year - dateOfBirth.Year;

            if (dateOfBirth > currentDate.AddYears(-age))
                age--;

            return age;
        }
    }
}