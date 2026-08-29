using FluentValidation;
using NeoVoting.Application.ResponseDTOs.AdminDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Validators.AdminDTOs
{
    public class SystemAuditLog_ResponseDTOValidator : AbstractValidator<SystemAuditLog_ResponseDTO>
    {
        public SystemAuditLog_ResponseDTOValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.TimestampUTC).NotNull();
            RuleFor(x => x.ActionType).NotNull();
            RuleFor(x => x.Details).NotEmpty();
            RuleFor(x => x.AdminId).NotNull();
            RuleFor(x => x.Username).NotEmpty();
        }
    }
}
