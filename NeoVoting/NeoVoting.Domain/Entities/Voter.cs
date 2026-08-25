using NeoVoting.Domain.Enums;
using NeoVoting.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public bool HasVoted { get; private set; }

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
                UserId = userId,
                HasVoted = false
            };
        }


        // when creating a new candidate or voter
        // user enter his private data (National ID + Token) to verify his identity
        // the government system will generate a hash of these data and store it in the database

        // in case user want to reset his password, he will enter his private data again to verify his identity
        // in some cases, the private data may change (e.g., the token may be updated), so the government system will generate a new hash and update it in the database
        // for that we need to update the verification hash in the database when the user reset his password

        // so we guarantee in all cases that the verification hash in the database is always up to date with the latest private data of the user

        public void UpdateFields_AtUserPasswordReset(

            string firstName,
            string lastName,
            DateOnly dateOfBirth,
            char gender,
            GovernorateIdEnum governorate,
            string verificationHash
            )
        {
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            Governorate = governorate;
            VerificationHash = verificationHash;
        }

        public void MarkAsVoted()
        {
            if(HasVoted)
            {
                throw new InvalidOperationException("This voter has already voted.");
            }

            HasVoted = true;

        }
    }
}
