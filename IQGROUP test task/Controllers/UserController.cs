using IQGROUP_test_task.Models;
using Microsoft.AspNetCore.Mvc;

namespace IQGROUP_test_task.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : Controller
    {
        IUserService _userService;
        ITokenService _tokenService;
        ILogger<UserController> _logger;

        public UserController(IUserService userService, ITokenService tokenService, ILogger<UserController> logger)
        {
            _userService = userService;
            _tokenService = tokenService;
            _logger = logger;
        }
        // Получить всех пользователей
        // GET: api/user

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var users = await _userService.FindAllAsync();
                // log
                _logger.LogInformation("Get method is success!");
                return Ok(users);
            }
            catch
            {
                // log
                return Ok();
            }
        }
        // Получить пользователя по его id
        // GET: api/user/{id}

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            try
            {
                // log
                _logger.LogInformation("Get method is success!");
                return Ok();
            }
            catch
            {
                // log
                return Ok();
            }
        }
        // Обработать запрос на аутентификацию пользователя
        // POST: api/user/auth

        [HttpPost]
        [Route("auth")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] AuthUserModel authUser)
        {
            if(authUser == null)
            {
                // log
                return BadRequest();
            }
            else if(authUser.Email is null || authUser.Password is null)
            {
                // log
                return BadRequest();
            }
            else
            {
                UserModel user = await _userService.FindAsync(authUser);
                if(user is null)
                {
                    // log
                    return NotFound();
                }
                else
                {
                    // log
                    string jwtToken = _tokenService.GenerateJwtToken(user);
                    if (jwtToken is null)
                    {
                        // log
                        return Ok("Ошибка аутентификации из-за сервера");
                    }
                    Response.Headers.Add("Authorization", $"Bearer {jwtToken}");

                    string refreshToken = _tokenService.GenerateRefreshToken();

                    Response.Cookies.Append("RefreshToken", refreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddHours(1)
                    });

                    AuthResponseModel response = new AuthResponseModel
                    {
                        UserId = user._id,
                        UserEmail = user.Email,
                        UserLogin = user.Login,
                        JwtToken = jwtToken,
                        RefreshToken = refreshToken
                    };

                    return Ok(response);
                }
            }
        }
        // Обработать запрос на регистрацию нового пользователя
        // POST: api/user/reg

        [HttpPost]
        [Route("reg")]
        public async Task<IActionResult> AddAsync([FromBody] RegUserModel regUser)
        {
            if(regUser == null)
            {
                // log
                return BadRequest();
            }

            else if(regUser.Login is null || regUser.Email is null || regUser.Password is null || regUser.ConfirmPassword is null)
            {
                // log
                return BadRequest();
            }
            else
            {
                UserModel user = new UserModel(Guid.NewGuid().ToString(), regUser.Login, regUser.Email, regUser.Password);
                try
                {
                    await _userService.InsertOneAsync(user);

                    // log
                    string token = _tokenService.GenerateJwtToken(user);
                    if (token is null)
                    {
                        // log
                        return Ok("Ошибка регистрации из-за сервера");
                    }
                    Response.Headers.Add("Authorization", $"Bearer {token}");

                    string refreshToken = _tokenService.GenerateRefreshToken();

                    Response.Cookies.Append("RefreshToken", refreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddHours(1)
                    });

                    return Ok(user);
                    // log
                }
                catch (Exception ex)
                {
                    // log
                    return Ok(ex.Message);
                }
            }
        }
        // Удалить пользователя по его id
        //POST: api/user/rmv/{id

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> RemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                // log
                return BadRequest();
            }
            try
            {
                // log
                await _userService.DeleteAsync(id);
                _logger.LogInformation($"Delete method is success {id}!");
                return Ok();
            }
            catch
            {
                // log
                return Ok();
            }
        }
        // Обновить данные пользователя
        //POST: api/user/upd

       [HttpPost]
       [Route("upd")]
        public async Task<IActionResult> UpdateAsync([FromBody] UserModel user)
        {
            try
            {
                // log
                _logger.LogInformation("Get method is success!");
                return Ok();
            }
            catch
            {
                // log
                return Ok();
            }
        }
        // Проверка почты на уникальность
        //POST: api/user/check-email

        [HttpPost]
        [Route("check-email")]
        public async Task<IActionResult> CheckEmailAvailabilityAsync([FromBody] string email)
        {
            if (email == null)
            {
                // log
                return Ok(false);
            }
            try
            {
                // log
                bool isAvailable = await _userService.CheckEmailAvailabilityAsync(email);
                return Ok(isAvailable);
            }
            catch (Exception ex)
            {
                // log
                return Ok(ex.Message);
            }
        }
        // Проверка логина на уникальность
        //POST: api/user/check-login

        [HttpPost]
        [Route("check-login")]
        public async Task<IActionResult> CheckLoginAvailabilityAsync([FromBody] string login)
        {
            if (login == null)
            {
                // log
                return Ok(false);
            }
            try
            {
                // log
                bool isAvailable = await _userService.CheckLoginAvailabilityAsync(login);
                return Ok(isAvailable);
            }
            catch (Exception ex)
            {
                // log
                return Ok(ex.Message);
            }
        }


    }
}
