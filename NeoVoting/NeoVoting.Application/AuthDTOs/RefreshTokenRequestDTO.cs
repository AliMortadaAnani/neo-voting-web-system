using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.AuthDTOs
{
    public class RefreshTokenRequestDTO
    {
        public string? RefreshToken { get; set; }

        public string? AccessToken { get; set; }
    }
}
