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
            RuleFor(x => x.File).NotNull();
        }
    }
}
