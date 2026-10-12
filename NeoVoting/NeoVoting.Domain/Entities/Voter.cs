using NeoVoting.Domain.Enums;

namespace NeoVoting.Domain.Entities
{
    public class Voter
    {
        public int Id { get; private set; } // auto-incremented by DB
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public DateOnly DateOfBirth { get; private set; }
        public GovernorateIdEnum Governorate { get; private set; }
        public char Gender { get; private set; } // 'M'/ 'm' or 'F' / 'f'

        public string VerificationHash { get; private set; } = string.Empty; // Hash of the National ID + the VotingToken retrieved from GovernmentSystem API

        public int UserId { get; private set; }

        public ApplicationUser User { get; private set; }

        //  public bool HasVoted { get; private set; }

        public ICollection<EventParticipation> EventParticipations { get; private set; } = new List<EventParticipation>();

        private Voter()
        {
            User = null!;
        }

        public static Voter Create(

            string firstName,
            string lastName,
            DateOnly dateOfBirth,
            char gender,
            GovernorateIdEnum governorate,
            string verificationHash,
            int userId

            )
        {
            return new Voter
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