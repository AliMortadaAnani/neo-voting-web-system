using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using NeoVoting.Application.RequestDTOs.CandidateDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.ResultErrorDomain;

namespace NeoVoting.Application.Services
{
    public class LocalFileServices : IFileServices
    {
        private readonly IWebHostEnvironment _environment;

        public LocalFileServices(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<Result<string>> SaveFileAsync(CandidateProfileUploadImage_RequestDTO fileDto)
        {
            if (fileDto == null) throw new ArgumentNullException(nameof(fileDto));
            if (fileDto.File == null) throw new ArgumentNullException(nameof(fileDto.File));

            // 1. Validate Extension
            var ext = Path.GetExtension(fileDto.File.FileName).ToLowerInvariant();
            

            // 2. Create unique filename (Guid) to prevent overwriting and security issues
            var fileName = $"{Guid.NewGuid()}{ext}";

            //wwwroot should be created
            // 3. Define path (e.g., wwwroot/uploads/profiles)
            var folderPath = Path.Combine(_environment.WebRootPath, "uploads", "profiles");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);

            // 4. Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileDto.File.CopyToAsync(stream);
            }
            string relativeUrl = $"/uploads/profiles/{fileName}";
            // 5. Return relative URL for database storage
            return Result<string>.Success(relativeUrl);
        }

        public void DeleteFile(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl)) return;

            // Convert URL back to local path logic if needed
            var filePath = Path.Combine(_environment.WebRootPath, fileUrl.TrimStart('/'));
            if (File.Exists(filePath)) File.Delete(filePath);
        }

      
    }
}