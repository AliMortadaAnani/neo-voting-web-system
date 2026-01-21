using FluentValidation;
using NeoVoting.Application.RequestDTOs;

namespace NeoVoting.Application.Validators
{
    public class CandidateProfileUploadImage_RequestDTO_Validator : AbstractValidator<CandidateProfileUploadImage_RequestDTO>
    {
        // Allowed image extensions
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };
        
        // Allowed MIME types
        private static readonly string[] AllowedContentTypes = 
        { 
            "image/jpeg", 
            "image/png"
        };

        // Max file size: 5 MB
        private const long MaxFileSizeInBytes = 5 * 1024 * 1024;

        public CandidateProfileUploadImage_RequestDTO_Validator()
        {
            RuleFor(x => x.File)
                .NotNull().WithMessage("Profile image file is required.")
                .Must(file => file != null && file.Length > 0).WithMessage("File cannot be empty.")
                .Must(file => file != null && file.Length <= MaxFileSizeInBytes)
                    .WithMessage($"File size cannot exceed {MaxFileSizeInBytes / (1024 * 1024)} MB.")
                .Must(HasValidExtension)
                    .WithMessage($"Invalid file extension. Allowed extensions: {string.Join(", ", AllowedExtensions)}")
                .Must(HasValidContentType)
                    .WithMessage($"Invalid file type. Allowed types: {string.Join(", ", AllowedContentTypes)}");
        }

        private static bool HasValidExtension(Microsoft.AspNetCore.Http.IFormFile? file)
        {
            if (file == null || string.IsNullOrEmpty(file.FileName))
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return AllowedExtensions.Contains(extension);
        }

        private static bool HasValidContentType(Microsoft.AspNetCore.Http.IFormFile? file)
        {
            if (file == null || string.IsNullOrEmpty(file.ContentType))
                return false;

            return AllowedContentTypes.Contains(file.ContentType.ToLowerInvariant());
        }
    }
}
