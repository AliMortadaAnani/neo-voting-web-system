using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.AuthDTOs
{
    public class AuthenticationResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiration { get; set; }

        public DateTime RefreshTokenExpiration { get; set; }

        // --- User Info (For the UI) ---
        public Guid? Id { get; set; }
        public string? UserName { get; set; } 
        public string? FirstName { get; set; } 
        public string? LastName { get; set; } 
        
        public int? GovernorateId { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public char? Gender { get; set; }
        public string? Role { get; set; }



    }
}
