using IQGROUP_test_task.Models;
using Microsoft.AspNetCore.Mvc;

namespace IQGROUP_test_task.Controllers
{
    [ApiController]
    [Route("api/token")]
    public class JwtTokenController : ControllerBase
    {
        ITokenService _tokenService;

        public JwtTokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }


        // GET: api/token/refresh

        [HttpGet]
        [Route("refresh")]
        public IActionResult UpdateTokenAsync()
        {
            try
            {
                string jwtToken = Request.Headers.Authorization;
                Request.Cookies.TryGetValue("refresh", out string refreshToken);

                if(string.IsNullOrEmpty(refreshToken))
                {
                    // log
                    return Unauthorized();
                }
                else
                {
                    // log
                    return null;
                }
            }
            catch (Exception ex)
            {
                //log
                return Ok();
            }


        }

        
    }
}
