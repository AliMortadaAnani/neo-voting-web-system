//using System.Security.Claims;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace GovernmentSystem.API.API.Controllers
//{
//    [Route("api/debug")]
//    [ApiController]
//    public class DebugController : ControllerBase
//    {
//        // [Authorize] ensures you must be logged in with a cookie to hit this
//        [HttpGet("claims")]
//        [Authorize]
//        public IActionResult GetMyClaims()
//        {
//            // HttpContext.User holds the ClaimsPrincipal built from your auth cookie
//            var claims = User.Claims.Select(c => new
//            {
//                c.Type,
//                c.Value,
//                c.ValueType
//            }).ToList();

//            // Get the identity authentication status and type
//            var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
//            var authType = User.Identity?.AuthenticationType;

//            return Ok(new
//            {
//                IsAuthenticated = isAuthenticated,
//                AuthenticationType = authType,
//                TotalClaims = claims.Count,
//                Claims = claims
//            });
//        }
//    }
//}