using GovernmentSystem.API.Domain.Shared;
using Microsoft.AspNetCore.Identity;

namespace GovernmentSystem.API.Domain.Entities
{
    public class ApplicationRole : IdentityRole<int>
    {
        // this private constructor is needed for EF Core as AI/Search recommended

        // IN CASE : we didn't assign a private/public constructor 
        // => C# by default will create a public parameterless constructor, which EF Core will use to create instances of the entity.
        private ApplicationRole() : base() { }

        private ApplicationRole(string roleName) : base(roleName) { }

        public static ApplicationRole CreateAdminRole()
        {
            return new ApplicationRole(RoleTypesEnum.Admin.ToString());
        }

    }
}