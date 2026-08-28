using Microsoft.AspNetCore.Http;

namespace NeoVoting.Application.RequestDTOs
{
    public class CandidateProfileUploadImage_RequestDTO
    {
        public IFormFile? File { get; set; }
    }
}