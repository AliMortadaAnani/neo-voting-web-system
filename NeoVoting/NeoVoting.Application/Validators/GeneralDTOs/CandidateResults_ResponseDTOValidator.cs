using FluentValidation;
using NeoVoting.Application.ResponseDTOs.GeneralDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Validators.GeneralDTOs
{
    public class CandidateResults_ResponseDTOValidator : AbstractValidator<CandidateResults_ResponseDTO>
    {
        public CandidateResults_ResponseDTOValidator()
        {
            RuleFor(x => x.CandidateProfileId).NotNull();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Governorate).NotNull();
            // RuleFor(x => x.ProfilePhotoFilename).NotEmpty(); // Optional: Uncomment if you want to enforce a non-empty profile photo filename
            RuleFor(x => x.VoteCount).NotNull();
        }

     }
}
