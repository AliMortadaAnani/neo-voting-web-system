using FluentValidation;
using NeoVoting.Application.ResponseDTOs.AdminDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Validators.AdminDTOs
{
    public class Poll_ResponseDTOValidator : AbstractValidator<Poll_ResponseDTO>
    {
        public Poll_ResponseDTOValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Question).NotEmpty();
            RuleFor(x => x.StartDate).NotNull();
            RuleFor(x => x.EndDate).NotNull();
            RuleFor(x => x.Answers).NotNull();
        }
    }

    public class PollAnswerListDTOValidator : AbstractValidator<PollAnswerListDTO>
    {
        public PollAnswerListDTOValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.Answer).NotEmpty();
        }
    }
}
