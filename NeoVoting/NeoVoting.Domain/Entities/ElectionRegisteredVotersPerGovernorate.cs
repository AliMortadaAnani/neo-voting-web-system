using NeoVoting.Domain.Enums;
using System;

namespace NeoVoting.Domain.Entities
{
    /// <summary>
    /// Represents a snapshot of registered voters per governorate for a specific election,
    /// with gender and age group breakdown at the moment of election completion.
    /// </summary>
    public class ElectionRegisteredVotersPerGovernorate
    {
        public int Id { get; private set; }

        /// <summary>
        /// Election to which this snapshot pertains.
        /// </summary>
        public Guid ElectionId { get; private set; }


        // governorateId is nullable because it can represent the total registered voters across all governorates when null

        /// <summary>
        /// Governorate to which this statistical record belongs.
        /// </summary>
        public int? GovernorateId { get; private set; }

        /// <summary>
        /// Total number of registered voters for this governorate at the snapshot moment.
        /// </summary>
        public int RegisteredVotersCount { get; private set; }

        /// <summary>
        /// Number of registered male voters in the governorate.
        /// </summary>
        public int RegisteredMalesCount { get; private set; }

        /// <summary>
        /// Number of registered female voters in the governorate.
        /// </summary>
        public int RegisteredFemalesCount { get; private set; }

        /// <summary>
        /// Number of registered voters aged 18 to 29.
        /// </summary>
        public int RegisteredAge18To29Count { get; private set; }

        /// <summary>
        /// Number of registered voters aged 30 to 45.
        /// </summary>
        public int RegisteredAge30To45Count { get; private set; }

        /// <summary>
        /// Number of registered voters aged 46 to 64.
        /// </summary>
        public int RegisteredAge46To64Count { get; private set; }

        /// <summary>
        /// Number of registered voters aged 65 and over.
        /// </summary>
        public int RegisteredAge65AndOverCount { get; private set; }

        // Navigation properties (optional depending on design)
         public Election Election { get; private set; }
         public Governorate? Governorate { get; private set; }

        // Private constructor for EF Core
        private ElectionRegisteredVotersPerGovernorate() 
        {
        Election = null!;
        
        }

        public static ElectionRegisteredVotersPerGovernorate Create(
            Guid electionId,
            int? governorateId,
            int registeredVotersCount,
            int registeredMalesCount,
            int registeredFemalesCount,
            int registeredAge18To29Count,
            int registeredAge30To45Count,
            int registeredAge46To64Count,
            int registeredAge65AndOverCount)
        {


            // You may add validation logic here if needed.

            // Validate counts
            ValidateCounts(
                registeredVotersCount,
                registeredMalesCount,
                registeredFemalesCount,
                registeredAge18To29Count,
                registeredAge30To45Count,
                registeredAge46To64Count,
                registeredAge65AndOverCount);

            if(electionId == Guid.Empty)
                throw new ArgumentException("ElectionId cannot be empty.", nameof(electionId));

            if (governorateId.HasValue)
            {
                if (!Enum.IsDefined(typeof(GovernoratesEnum), governorateId.Value))
                    throw new ArgumentException($"Invalid governorate id: {governorateId}", nameof(governorateId));
            }


            return new ElectionRegisteredVotersPerGovernorate
            {
                ElectionId = electionId,
                GovernorateId = governorateId,
                RegisteredVotersCount = registeredVotersCount,
                RegisteredMalesCount = registeredMalesCount,
                RegisteredFemalesCount = registeredFemalesCount,
                RegisteredAge18To29Count = registeredAge18To29Count,
                RegisteredAge30To45Count = registeredAge30To45Count,
                RegisteredAge46To64Count = registeredAge46To64Count,
                RegisteredAge65AndOverCount = registeredAge65AndOverCount
            };
        }

        private static void ValidateCounts(
    int registeredVotersCount,
    int registeredMalesCount,
    int registeredFemalesCount,
    int registeredAge18To29Count,
    int registeredAge30To45Count,
    int registeredAge46To64Count,
    int registeredAge65AndOverCount)
        {
            if (registeredVotersCount < 0 ||
                registeredMalesCount < 0 ||
                registeredFemalesCount < 0 ||
                registeredAge18To29Count < 0 ||
                registeredAge30To45Count < 0 ||
                registeredAge46To64Count < 0 ||
                registeredAge65AndOverCount < 0)
                throw new ArgumentException("Registered voter counts cannot be negative.");
        }
    }
}