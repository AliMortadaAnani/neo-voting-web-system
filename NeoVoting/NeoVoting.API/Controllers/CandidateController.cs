using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.Enums;

namespace NeoVoting.API.Controllers
{
    /// <summary>
    /// Candidate operations for managing their election profile.
    /// </summary>
    [Authorize(Roles = nameof(RoleTypesEnum.Candidate))]
    public class CandidateController : ApiController
    {
       
    }
}