using Microsoft.AspNetCore.Identity;

namespace GovernmentSystem.API.Domain.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        private ApplicationUser()
        { }

        public static ApplicationUser CreateAdminUser(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("Username is required for an admin account.", nameof(userName));
            }

            return new ApplicationUser
            {
                UserName = userName
            };
        }

    }
}