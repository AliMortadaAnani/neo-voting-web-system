using Microsoft.AspNetCore.Http;

namespace NeoVoting.Application.RequestDTOs.CandidateDTOs
{
    public class CandidateProfileUploadImage_RequestDTO
    {
        public IFormFile? File { get; set; }
    }
}