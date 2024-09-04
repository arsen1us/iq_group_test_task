using IQGROUP_test_task.Services;
using Microsoft.AspNetCore.Mvc;

namespace IQGROUP_test_task.Controllers
{
    [ApiController]
    [Route("api/token")]
    public class TokenController : Controller
    {
        ITokenService _tokenService;
        ILogger<UserController> _logger;
        IDateTimeService _dateTimeService;

        public TokenController(ITokenService tokenService, ILogger<UserController> logger, IDateTimeService dateTimeService)
        {
            _tokenService = tokenService;
            _logger = logger;
        }
        // GET: api/token/refresh-token

        [HttpGet]
        [Route("refresh-token")]
        public async Task<IActionResult> UpdateToken()
        {
            string timestamp = _dateTimeService.GetDateTimeNow();
            try
            {
                var jwtToken = HttpContext.Request.Headers["Authorization"].ToString();
                var refreshToken = HttpContext.Request.Cookies["RefreshToken"];

                string newJwtToken = await _tokenService.UpdateJwtTokenAsync(jwtToken);
                string newRefreshToken = _tokenService.GenerateRefreshToken();

                Response.Headers.Add("Authorization", $"Bearer {newJwtToken}");

                Response.Cookies.Append("RefreshToken", newRefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                });

                return Ok("Bearer " + newJwtToken);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"[{timestamp}] Произошла внутрянняя ошибка сервера. Подробности: {ex.Message}");
            }

        }

    }
}
