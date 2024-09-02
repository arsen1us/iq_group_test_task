using IQGROUP_test_task.Models;
using Microsoft.AspNetCore.Mvc;

namespace IQGROUP_test_task.Controllers
{
    [ApiController]
    [Route("api/token")]
    public class JwtTokenController : ControllerBase
    {
        // GET: api/token/refresh

        [HttpGet]
        [Route("refresh")]
        public IActionResult UpdateTokenAsync()
        {
            return Ok
        }

        
    }
}
