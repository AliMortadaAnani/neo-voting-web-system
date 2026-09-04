using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.Enums;

namespace NeoVoting.API.Controllers
{
    /// <summary>
    /// Administrative operations for managing elections and viewing system audit logs.
    /// </summary>
    /// 


    [Authorize(Roles = nameof(RoleTypesEnum.Admin))]
    public class AdminController : ApiController
    {
        private readonly IAdminServices _adminServices;

        public AdminController(IAdminServices adminServices)
        {
            _adminServices = adminServices;
        }


    }
}