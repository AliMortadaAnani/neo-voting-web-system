using Microsoft.AspNetCore.Identity;
using NeoVoting.Domain.Enums;

namespace NeoVoting.Domain.Entities
{
    public class ApplicationRole : IdentityRole<int>
    {
       
        private ApplicationRole() : base() { }

        
        private ApplicationRole(string roleName) : base(roleName) { }

        public static ApplicationRole CreateAdminRole()
        {
            return new ApplicationRole(RoleTypesEnum.Admin.ToString());
        }

        public static ApplicationRole CreateVoterRole()
        {
            return new ApplicationRole(RoleTypesEnum.Voter.ToString());
        }

        public static ApplicationRole CreateCandidateRole()
        {
            return new ApplicationRole(RoleTypesEnum.Candidate.ToString());
        }
    }
}