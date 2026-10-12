using NeoVoting.Domain.Enums;

namespace NeoVoting.Domain.Entities
{
    public class Candidate
    {
        public int Id { get; private set; } // auto-incremented by DB
        public string FirstName { get; private set; } = string.Empty; //from gov api
        public string LastName { get; private set; } = string.Empty; // from gov api
        public DateOnly DateOfBirth { get; private set; } // from gov api
        public GovernorateIdEnum Governorate { get; private set; } // from gov api
        public char Gender { get; private set; } // from gov api

        public string VerificationHash { get; private set; } = string.Empty; // Hash of the National ID + the NominationToken retrieved from GovernmentSystem API

        public int UserId { get; private set; }

        public ApplicationUser User { get; private set; }

        public ICollection<CandidateProfile> CandidateProfiles { get; private set; }
             = new List<CandidateProfile>();

        private Candidate()
        {
            User = null!;
        }

        public static Candidate Create(

            string firstName,
            string lastName,
            DateOnly dateOfBirth,
            char gender,
            GovernorateIdEnum governorate,
            string verificationHash,
            int userId

            )
        {
            return new Candidate
            {
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Gender = gender,
                Governorate = governorate,
                VerificationHash = verificationHash,
                UserId = userId
            };
        }

        // when creating a new candidate or voter
        // user enter his private data (National ID + Token) to verify his identity
        // the government system will generate a hash of these data and store it in the database

        
    }
}