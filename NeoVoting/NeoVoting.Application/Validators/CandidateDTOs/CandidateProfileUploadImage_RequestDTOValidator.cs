using FluentValidation;
using NeoVoting.Application.RequestDTOs.CandidateDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Validators.CandidateDTOs
{
    public class CandidateProfileUploadImage_RequestDTOValidator : AbstractValidator<CandidateProfileUploadImage_RequestDTO>
    {
        public CandidateProfileUploadImage_RequestDTOValidator()
        {
            RuleFor(f => f.File)
             .NotNull().WithMessage("File is required.")
            .Must(f => f != null && f.Length > 0).WithMessage("File cannot be empty.")
            .Must(f => f != null && f.Length <= 5 * 1024 * 1024).WithMessage("File size must be <= 5 MB.")
            .Must(f => f != null && new[] { ".jpg", ".png", ".jpeg" }
                .Contains(Path.GetExtension(f.FileName).ToLower()))
            .WithMessage("Only .jpg, .png, or .jpeg files are allowed.")
            .Must(f => f != null && (f.ContentType == "image/jpeg" ||  f.ContentType == "image/png" || f.ContentType == "application/pdf"))
            .WithMessage("Invalid file type.");
        }
    }
}
