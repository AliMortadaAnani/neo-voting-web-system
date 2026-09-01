using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.Enums;

namespace NeoVoting.API.Controllers
{

    [Authorize(Roles = nameof(RoleTypesEnum.Voter))]
    public class VoterController : ApiController
    {
    
    }
}