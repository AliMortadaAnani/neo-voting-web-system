using NeoVoting.Domain.Enums;
using NeoVoting.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Domain.Entities
{
    public class Candidate
    {
        public int Id { get; private set; } // auto-incremented by DB
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public DateOnly DateOfBirth { get; private set; }
        public GovernorateIdEnum Governorate { get; private set; }
        public char Gender { get; private set; } // 'M'/ 'm' or 'F' / 'f'

        public string VerificationHash { get; private set; } = string.Empty; // Hash of the National ID + the NominationToken retrieved from GovernmentSystem API

        public int UserId { get; private set; }

        public ApplicationUser User { get; private set; }

       public ICollection<CandidateProfile> candidateProfiles { get; private set; } 
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


        public void UpdateVerificationHash_AtUserPasswordReset(
          string newVerificationHash)
        {
            if (string.IsNullOrWhiteSpace(newVerificationHash))
                throw new ArgumentException("New verification hash must not be null, empty, or whitespace.", nameof(newVerificationHash));

            VerificationHash = newVerificationHash;
        }

       

    }
}
