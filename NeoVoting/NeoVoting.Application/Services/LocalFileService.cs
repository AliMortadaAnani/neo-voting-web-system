using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Services
{
    public class LocalFileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public LocalFileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<Result<string>> SaveFileAsync(IFormFile file, string[] allowedExtensions)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            // 1. Validate Extension
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
            {
                throw new ArgumentException($"Invalid file type. Allowed: {string.Join(",", allowedExtensions)}");
            }

            // 2. Create unique filename (Guid) to prevent overwriting and security issues
            var fileName = $"{Guid.NewGuid()}{ext}";

            // 3. Define path (e.g., wwwroot/uploads/profiles)
            var folderPath = Path.Combine(_environment.WebRootPath, "uploads", "profiles");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);

            // 4. Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
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

        /*
         try 
 {
     // 1. Save NEW image FIRST
     // If this fails, nothing changes. User is safe.
     var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
     var newImageUrl = await _fileService.SaveFileAsync(dto.File, allowedExtensions);

     // 2. Keep a reference to the OLD URL
     var oldImageUrl = enrollment.CourseSpecificAvatarUrl;

     // 3. Update Database
     // If this fails, we have an "orphan" file (newImageUrl), but the user's current profile works.
     enrollment.CourseSpecificAvatarUrl = newImageUrl;
     await _context.SaveChangesAsync();

     // 4. NOW it is safe to delete the old file
     // We wrap this in a separate try/catch because if this fails, 
     // we don't want to crash the request. The user updated successfully, 
     // we just failed to clean up garbage.
     if (!string.IsNullOrEmpty(oldImageUrl))
     {
         try 
         { 
             _fileService.DeleteFile(oldImageUrl); 
         } 
         catch 
         { 
             // Log this: "Failed to delete old file: {oldImageUrl}"
             // Do NOT throw error to client.
         }
     }

     return Ok(new { url = newImageUrl });
 }
 catch (Exception ex)
 {
     // If Step 1 or 3 failed, we return error, and the user's old profile is still valid.
     return BadRequest(ex.Message);
 } 

         */
    }
}