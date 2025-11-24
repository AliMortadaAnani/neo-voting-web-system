using GovernmentSystem.API.Application.RequestDTOs;
using GovernmentSystem.API.Application.ResponseDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using GovernmentSystem.API.Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace GovernmentSystem.API.API.Controllers
{
    
    [Route("api/[controller]")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public class VoterController : ApiController
    {
        private readonly IVoterServices _voterServices;
        public VoterController(IVoterServices voterServices)
        {
            _voterServices = voterServices;
        }


        
      
        [HttpPost("add-voter")]
        [ProducesResponseType(typeof(VoterResponseDTO), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddVoter(CreateVoterRequestDTO request)
        {
            var result = await _voterServices.AddVoterAsync(request);
            return HandleResult(result);
        }
    }
}
