using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace NeoVoting.Application.DTOs.Validators
{
    /// <summary>
    /// A custom validator to apply rules directly to an IFormFile object.
    /// This would be used manually inside the controller or service.
    /// </summary>
    public class ProfilePhotoUploadValidator : AbstractValidator<IFormFile>
    {
        public ProfilePhotoUploadValidator()
        {
            // Rule for file existence and size
            RuleFor(x => x.Length)
                .NotNull()
                .LessThanOrEqualTo(5 * 1024 * 1024) // 5 MB
                .WithMessage("The image file size cannot exceed 5 MB.");

            // Rule for file content type
            RuleFor(x => x.ContentType)
                .NotNull()
                .Must(x => x.Equals("image/jpeg") || x.Equals("image/png") || x.Equals("image/gif"))
                .WithMessage("Please upload a valid image file (JPG, PNG, or GIF).");

            // --- Optional Suggested Rule ---
            // A rule to check the file extension as a fallback.

            RuleFor(x => x.FileName)
                .Must(x => new[] { ".jpg", ".jpeg", ".png", ".gif" }.Contains(Path.GetExtension(x).ToLower()))
                .WithMessage("Invalid file extension. Only JPG, PNG, and GIF are allowed.");
        }
    }
}