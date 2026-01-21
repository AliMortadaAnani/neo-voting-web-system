using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.RequestDTOs
{
    public class CandidateProfileUploadImage_RequestDTO
    {
        
        public required IFormFile File { get; set; }
    }
}
