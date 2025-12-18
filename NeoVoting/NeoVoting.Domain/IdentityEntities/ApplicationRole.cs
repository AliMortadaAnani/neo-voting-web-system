using Microsoft.AspNetCore.Identity;
using NeoVoting.Domain.Enums;

namespace NeoVoting.Domain.IdentityEntities
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        // --- Constructor ---

        /// <summary>
        /// A private constructor to force all role creation through the controlled factory methods.
        /// EF Core can use this to materialize objects from the database.
        /// </summary>
        private ApplicationRole() : base() { }

        /// <summary>
        /// A private constructor that takes the role name.
        /// </summary>
        private ApplicationRole(string roleName) : base(roleName) { }


        // --- Factory Methods ---

        /// <summary>
        /// Factory for creating the Administrator role.
        /// </summary>
        public static ApplicationRole CreateAdminRole()
        {
            var roleName = RoleTypesEnum.Admin.ToString();

            var adminRole = new ApplicationRole(roleName)
            {
                // The Id is generated here, making the object complete.
                Id = Guid.NewGuid(),
                // The NormalizedName is important for Identity's lookups.
                NormalizedName = roleName.ToUpperInvariant()
            };

            return adminRole;
        }

        /// <summary>
        /// Factory for creating the Voter role.
        /// </summary>
        public static ApplicationRole CreateVoterRole()
        {
            var roleName = RoleTypesEnum.Voter.ToString();

            var voterRole = new ApplicationRole(roleName)
            {
                Id = Guid.NewGuid(),
                NormalizedName = roleName.ToUpperInvariant()
            };

            return voterRole;
        }

        /// <summary>
        /// Factory for creating the Candidate role.
        /// </summary>
        public static ApplicationRole CreateCandidateRole()
        {
            var roleName = RoleTypesEnum.Candidate.ToString();

            var candidateRole = new ApplicationRole(roleName)
            {
                Id = Guid.NewGuid(),
                NormalizedName = roleName.ToUpperInvariant()
            };

            return candidateRole;
        }

        


    }
}
