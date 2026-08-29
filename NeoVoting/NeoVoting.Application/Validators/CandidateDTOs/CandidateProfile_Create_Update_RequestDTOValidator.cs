using FluentValidation;
using NeoVoting.Application.RequestDTOs.CandidateDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Validators.CandidateDTOs
{
    public class CandidateProfile_Create_Update_RequestDTOValidator : AbstractValidator<CandidateProfile_Create_Update_RequestDTO>
    {
        public CandidateProfile_Create_Update_RequestDTOValidator()
        {
            RuleFor(x => x.Goals).NotEmpty();
            RuleFor(x => x.NominationReasons).NotEmpty();
        }
    }
}
