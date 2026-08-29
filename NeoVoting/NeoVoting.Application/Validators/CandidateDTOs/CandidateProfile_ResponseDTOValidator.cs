using FluentValidation;
using NeoVoting.Application.ResponseDTOs.CandidateDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Validators.CandidateDTOs
{
    public class CandidateProfile_ResponseDTOValidator : AbstractValidator<CandidateProfile_ResponseDTO>
    {
        public CandidateProfile_ResponseDTOValidator()
        {
            RuleFor(x => x.CandidateProfileId).NotNull();
            RuleFor(x => x.Goals).NotEmpty();
            RuleFor(x => x.NominationReasons).NotEmpty();
            RuleFor(x => x.ProfilePhotoFilename).NotEmpty();
            RuleFor(x => x.ApplicationUserId).NotNull();
            RuleFor(x => x.CandidateId).NotNull();
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.DateOfBirth).NotNull();
            RuleFor(x => x.Gender).NotNull();
            RuleFor(x => x.Governorate).NotNull();
            RuleFor(x => x.ElectionId).NotNull();
            RuleFor(x => x.ElectionName).NotEmpty();
        }
    }
}
