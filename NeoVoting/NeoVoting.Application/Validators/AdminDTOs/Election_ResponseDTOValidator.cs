using FluentValidation;
using NeoVoting.Application.ResponseDTOs.AdminDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Validators.AdminDTOs
{
    public class Election_ResponseDTOValidator : AbstractValidator<Election_ResponseDTO>
    {
        public Election_ResponseDTOValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.NominationStartDate).NotNull();
            RuleFor(x => x.NominationEndDate).NotNull();
            RuleFor(x => x.VotingStartDate).NotNull();
            RuleFor(x => x.VotingEndDate).NotNull();
        }
    }
}
