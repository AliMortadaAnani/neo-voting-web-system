using NeoVoting.Domain.Enums;
using System;
using System.Text;

namespace NeoVoting.Domain.Entities
{
    public class ElectionAndPollStatistics
    {
        // Structural Keys (Keep these non-nullable for EF Core Identity/Relationships)
        public int Id { get; private set; }
        public int? ElectionId { get; private set; }
        public int? PollId { get; private set; }
        public GovernorateIdEnum? Governorate { get; private set; }

        // ==================================================================
        // 1. RAW COUNTS (Nullable)
        // ==================================================================

        // General
        public int? CandidatesNominatedCount { get; private set; }
        public int? RegisteredVotersCount { get; private set; }
        public int? ActualVotersCount { get; private set; }

        // Gender
        public int? RegisteredMalesCount { get; private set; }
        public int? RegisteredFemalesCount { get; private set; }
        public int? VotedMalesCount { get; private set; }
        public int? VotedFemalesCount { get; private set; }

        // Age Groups (Registered)
        public int? RegisteredAge18To29Count { get; private set; }
        public int? RegisteredAge30To45Count { get; private set; }
        public int? RegisteredAge46To64Count { get; private set; }
        public int? RegisteredAge65AndOverCount { get; private set; }

        // Age Groups (Voted)
        public int? VotedAge18To29Count { get; private set; }
        public int? VotedAge30To45Count { get; private set; }
        public int? VotedAge46To64Count { get; private set; }
        public int? VotedAge65AndOverCount { get; private set; }

        // ==================================================================
        // 2. PERCENTAGES (Nullable)
        // ==================================================================

        public double? ParticipationPercentage { get; private set; }

        // Gender Participation
        public double? MaleVotedOutOfRegisteredPercentage { get; private set; }
        public double? FemaleVotedOutOfRegisteredPercentage { get; private set; }
        public double? MaleVotedOutOfTotalVotedPercentage { get; private set; }
        public double? FemaleVotedOutOfTotalVotedPercentage { get; private set; }

        // Age Participation (Out of Registered)
        public double? Age18To29VotedOutOfRegisteredPercentage { get; private set; }
        public double? Age30To45VotedOutOfRegisteredPercentage { get; private set; }
        public double? Age46To64VotedOutOfRegisteredPercentage { get; private set; }
        public double? Age65AndOverVotedOutOfRegisteredPercentage { get; private set; }

        // Age Participation (Out of Total Voted)
        public double? Age18To29VotedOutOfTotalVotedPercentage { get; private set; }
        public double? Age30To45VotedOutOfTotalVotedPercentage { get; private set; }
        public double? Age46To64VotedOutOfTotalVotedPercentage { get; private set; }
        public double? Age65AndOverVotedOutOfTotalVotedPercentage { get; private set; }

        // Navigation properties
        public Election? Election { get; private set; }
        public Poll? Poll { get; private set; }

        // Private constructor for EF Core
        private ElectionAndPollStatistics() {
            Election = null!;
            Poll = null!;
        }

        // ==================================================================
        // 3. FACTORY METHOD
        // ==================================================================
        public static ElectionAndPollStatistics Create(
            int? electionId,
            int? pollId,
            GovernorateIdEnum? governorate,
            int? candidatesNominatedCount,
            // Counts
            int? registeredVotersCount,
            int? actualVotersCount,
            int? registeredMalesCount,
            int? registeredFemalesCount,
            int? votedMalesCount,
            int? votedFemalesCount,
            int? registeredAge18To29Count,
            int? registeredAge30To45Count,
            int? registeredAge46To64Count,
            int? registeredAge65AndOverCount,
            int? votedAge18To29Count,
            int? votedAge30To45Count,
            int? votedAge46To64Count,
            int? votedAge65AndOverCount,
            // Percentages
            double? participationPercentage,
            double? maleVotedOutOfRegisteredPercentage,
            double? femaleVotedOutOfRegisteredPercentage,
            double? maleVotedOutOfTotalVotedPercentage,
            double? femaleVotedOutOfTotalVotedPercentage,
            double? age18To29VotedOutOfRegisteredPercentage,
            double? age30To45VotedOutOfRegisteredPercentage,
            double? age46To64VotedOutOfRegisteredPercentage,
            double? age65AndOverVotedOutOfRegisteredPercentage,
            double? age18To29VotedOutOfTotalVotedPercentage,
            double? age30To45VotedOutOfTotalVotedPercentage,
            double? age46To64VotedOutOfTotalVotedPercentage,
            double? age65AndOverVotedOutOfTotalVotedPercentage
            )
        {
            // 1. Validate Non-Negativity Only (Ignores nulls)
            ValidateNonNegative(
                candidatesNominatedCount,
                registeredVotersCount, actualVotersCount,
                registeredMalesCount, registeredFemalesCount,
                votedMalesCount, votedFemalesCount,
                registeredAge18To29Count, registeredAge30To45Count, registeredAge46To64Count, registeredAge65AndOverCount,
                votedAge18To29Count, votedAge30To45Count, votedAge46To64Count, votedAge65AndOverCount,
                participationPercentage,
                maleVotedOutOfRegisteredPercentage, femaleVotedOutOfRegisteredPercentage,
                maleVotedOutOfTotalVotedPercentage, femaleVotedOutOfTotalVotedPercentage,
                age18To29VotedOutOfRegisteredPercentage, age30To45VotedOutOfRegisteredPercentage,
                age46To64VotedOutOfRegisteredPercentage, age65AndOverVotedOutOfRegisteredPercentage,
                age18To29VotedOutOfTotalVotedPercentage, age30To45VotedOutOfTotalVotedPercentage,
                age46To64VotedOutOfTotalVotedPercentage, age65AndOverVotedOutOfTotalVotedPercentage
            );

           
            if (governorate.HasValue && !Enum.IsDefined(typeof(GovernorateIdEnum), governorate.Value))
                throw new ArgumentException($"Invalid governorate id: {governorate}", nameof(governorate));

            // 2. Direct Assignment
            return new ElectionAndPollStatistics
            {
                ElectionId = electionId,
                PollId = pollId,
                Governorate = governorate,
                CandidatesNominatedCount = candidatesNominatedCount,

                RegisteredVotersCount = registeredVotersCount,
                ActualVotersCount = actualVotersCount,
                RegisteredMalesCount = registeredMalesCount,
                RegisteredFemalesCount = registeredFemalesCount,
                VotedMalesCount = votedMalesCount,
                VotedFemalesCount = votedFemalesCount,
                RegisteredAge18To29Count = registeredAge18To29Count,
                RegisteredAge30To45Count = registeredAge30To45Count,
                RegisteredAge46To64Count = registeredAge46To64Count,
                RegisteredAge65AndOverCount = registeredAge65AndOverCount,
                VotedAge18To29Count = votedAge18To29Count,
                VotedAge30To45Count = votedAge30To45Count,
                VotedAge46To64Count = votedAge46To64Count,
                VotedAge65AndOverCount = votedAge65AndOverCount,

                ParticipationPercentage = participationPercentage,
                MaleVotedOutOfRegisteredPercentage = maleVotedOutOfRegisteredPercentage,
                FemaleVotedOutOfRegisteredPercentage = femaleVotedOutOfRegisteredPercentage,
                MaleVotedOutOfTotalVotedPercentage = maleVotedOutOfTotalVotedPercentage,
                FemaleVotedOutOfTotalVotedPercentage = femaleVotedOutOfTotalVotedPercentage,
                Age18To29VotedOutOfRegisteredPercentage = age18To29VotedOutOfRegisteredPercentage,
                Age30To45VotedOutOfRegisteredPercentage = age30To45VotedOutOfRegisteredPercentage,
                Age46To64VotedOutOfRegisteredPercentage = age46To64VotedOutOfRegisteredPercentage,
                Age65AndOverVotedOutOfRegisteredPercentage = age65AndOverVotedOutOfRegisteredPercentage,
                Age18To29VotedOutOfTotalVotedPercentage = age18To29VotedOutOfTotalVotedPercentage,
                Age30To45VotedOutOfTotalVotedPercentage = age30To45VotedOutOfTotalVotedPercentage,
                Age46To64VotedOutOfTotalVotedPercentage = age46To64VotedOutOfTotalVotedPercentage,
                Age65AndOverVotedOutOfTotalVotedPercentage = age65AndOverVotedOutOfTotalVotedPercentage
            };
        }

        // Accepts double? because int? can implicitly cast to double?
        private static void ValidateNonNegative(params double?[] values)
        {
            foreach (var value in values)
            {
                // Only check if value has a value (not null) AND is negative
                if (value.HasValue && value.Value < 0)
                {
                    throw new ArgumentException("Election/Poll statistics cannot contain negative values.");
                }
            }
        }
    }
}