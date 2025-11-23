using GovernmentSystem.API.Application.RequestDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using Microsoft.AspNetCore.Mvc;

namespace GovernmentSystem.API.API.Controllers
{
    [Route("api/ttt/[controller]")]
    public class VoterController : ApiController
    {
        private readonly IVoterServices _voterServices;
        public VoterController(IVoterServices voterServices)
        {
            _voterServices = voterServices;
        }

        [HttpPost("add-voter")]
        public async Task<IActionResult> AddVoter(CreateVoterRequestDTO request)
        {
            var result = await _voterServices.AddVoterAsync(request);
            return HandleResult(result);
        }
    }
}
