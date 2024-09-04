using Amazon.Runtime;
using IQGROUP_test_task.Models;
using IQGROUP_test_task.Services;
using Microsoft.AspNetCore.Authorization;
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
        IDateTimeService _dateTimeService;

        public UserController(IUserService userService, ITokenService tokenService, ILogger<UserController> logger, IDateTimeService dateTimeService)
        {
            _userService = userService;
            _tokenService = tokenService;
            _logger = logger;
            _dateTimeService = dateTimeService;
        }
        // Получить всех пользователей
        // GET: api/user

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            string timestamp = _dateTimeService.GetDateTimeNow();
            string method = "GET";
            string queryString = "api/user";

            try
            {
                var users = await _userService.FindAllAsync();
                _logger.LogInformation($"INFO: [{timestamp}] Successfully received [{users.Count}] users. Method - [{method}]. Query string - [{queryString}]");
                return Ok(users);
            }
            catch(Exception ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] An error occurred: {ex.Message}. Method - [{method}]. Query string - [{queryString}]");
                return StatusCode(500, $"[{timestamp}] Произошла внутрянняя ошибка сервера. Подробности: {ex.Message}");
            }
        }
        // Получить пользователя по его id
        // GET: api/user/{id}

        [Authorize]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> FindByIdAsync(string id)
        {
            string timestamp = _dateTimeService.GetDateTimeNow();
            string method = "GET";
            string queryString = "api/user/{id}";

            if (string.IsNullOrEmpty(id))
            {
                _logger.LogError($"ERROR: [{timestamp}] Passed parameter [id] is null or empty. Method - [{method}]. Query string - [{queryString}]");
                return BadRequest($"[{timestamp}] Переданные параметры оказались пустыми или равными null");
            }
            try
            {
                var user = await _userService.FindByIdAsync(id);
                _logger.LogInformation($"INFO: [{timestamp}] Successfully received user with id - [{id}]. Method - [{method}]. Query string - [{queryString}]");
                return Ok(user);
            }
            catch(Exception ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] An error occurred: {ex.Message}. Method - [{method}]. Query string - [{queryString}]");
                return StatusCode(500, $"[{timestamp}] Произошла внутрянняя ошибка сервера. Подробности: {ex.Message}");
            }
        }
        // Обработать запрос на аутентификацию пользователя
        // POST: api/user/auth

        [HttpPost]
        [Route("auth")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] AuthUserModel authUser)
        {
            string timestamp = _dateTimeService.GetDateTimeNow();
            string method = "POST";
            string queryString = "api/user/auth";

            if (authUser == null)
            {
                _logger.LogError($"ERROR: [{timestamp}] Passed auth model authUser is null or empty. Method - [{method}]. Query string - [{queryString}]");
                return BadRequest($"[{timestamp}] Переданные параметры оказались пустыми или равными null");
            }
            else if(authUser.Email is null || authUser.Password is null)
            {
                _logger.LogError($"ERROR: [{timestamp}] Passed auth model parameters [authUser.Email and authUser.Password] is null or empty. Method - [{method}]. Query string - [{queryString}]");
                return BadRequest($"[{timestamp}] Переданные параметры оказались пустыми или равными null");
            }
            else
            {
                try
                {
                    UserModel user = await _userService.FindAsync(authUser);
                    if (user is null)
                    {
                        _logger.LogError($"ERROR: [{timestamp}] User with email - [{authUser.Email}] and password - [{authUser.Password}] NOT FOUND. Method - [{method}]. Query string - [{queryString}]");
                        return NotFound();
                    }
                    else
                    {
                        string jwtToken = _tokenService.GenerateJwtToken(user);
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
                        _logger.LogInformation($"INFO: [{timestamp}] User with id - [{user._id}] successfully logged in. Method - [{method}]. Query string - [{queryString}]");
                        return Ok(response);
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError($"ERROR: [{timestamp}] An error occurred: {ex.Message}. Method - [{method}]. Query string - [{queryString}]");
                    return StatusCode(500, $"[{timestamp}] Произошла внутрянняя ошибка сервера. Подробности: {ex.Message}");
                }
            }
        }
        // Обработать запрос на регистрацию нового пользователя
        // POST: api/user/reg
        
        [HttpPost]
        [Route("reg")]
        public async Task<IActionResult> AddAsync([FromBody] RegUserModel regUser)
        {
            string timestamp = _dateTimeService.GetDateTimeNow();
            string method = "POST";
            string queryString = "api/user/reg";

            if (regUser == null)
            {
                _logger.LogError($"[{timestamp}] Registration model [regUser] is null. Method - [{method}]. Query string - [{queryString}]");
                return BadRequest($"[{timestamp}] Переданные параметры оказались пустыми или равными null");
            }

            else if(regUser.Login is null 
                || regUser.Email is null 
                || regUser.Password is null 
                || regUser.ConfirmPassword is null)
            {
                _logger.LogError($"[{timestamp}] Registration model parameters is null. Method - [{method}]. Query string - [{queryString}]");
                return BadRequest($"[{timestamp}] Переданные параметры оказались пустыми или равными null");
            }
            else
            {
                UserModel user = new UserModel(Guid.NewGuid().ToString(), regUser.Login, regUser.Email, regUser.Password);
                try
                {
                    await _userService.InsertOneAsync(user);

                    string jwtToken = _tokenService.GenerateJwtToken(user);
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
                    _logger.LogInformation($"INFO: [{timestamp}] User with id - [{user._id}] successfully registered. Method - [{method}]. Query string - [{queryString}]");
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"ERROR: [{timestamp}] An error occurred: {ex.Message}. Method - [{method}]. Query string - [{queryString}]");
                    return StatusCode(500, $"[{timestamp}] Произошла внутрянняя ошибка сервера. Подробности: {ex.Message}");
                }
            }
        }
        // Удалить пользователя по его id
        //POST: api/user/rmv/{id}
        [Authorize]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> RemoveAsync(string id)
        {
            string timestamp = _dateTimeService.GetDateTimeNow();
            string method = "POST";
            string queryString = "api/user/rmv/{id}";

            if (string.IsNullOrEmpty(id))
            {
                _logger.LogError($"ERROR: [{timestamp}] Parameters id is null or empty. Method - [{method}]. Query string - [{queryString}]");
                return BadRequest($"[{timestamp}] Переданные параметры оказались пустыми или равными null");
            }
            try
            {
                await _userService.DeleteAsync(id);

                _logger.LogInformation($"INFO: [{timestamp}] User with id - [{id}] successfully deleted. Method - [{method}]. Query string - [{queryString}]");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] An error occurred: {ex.Message}. Method - [{method}]. Query string - [{queryString}]");
                return StatusCode(500, $"[{timestamp}] Произошла внутрянняя ошибка сервера. Подробности: {ex.Message}");
            }
        }
        // Обновить данные пользователя
        //POST: api/user/{id}

        [Authorize]
        [HttpPost]
        [Route("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] UserModel user)
        {
            string timestamp = _dateTimeService.GetDateTimeNow();
            string method = "POST";
            string queryString = "api/user/{id}";

            if (string.IsNullOrEmpty(id) || user is null)
            {
                return BadRequest($"[{timestamp}] Переданные параметры оказались пустыми или равными null");
            }
            try
            {
                await _userService.UpdateAsync(id, user);
                _logger.LogInformation($"INFO: [{timestamp}] User with id - [{id}] successfully updated. Method - [{method}]. Query string - [{queryString}]");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] An error occurred: {ex.Message}. Method - [{method}]. Query string - [{queryString}]");
                return StatusCode(500, $"[{timestamp}] Произошла внутрянняя ошибка сервера. Подробности: {ex.Message}");
            }
        }
        // Обновить данные пользователя и сгенерировать jwt-токен
        //POST: api/user/{id}

        [Authorize]
        [HttpPost]
        [Route("upd-with-jwt/{id}")]
        public async Task<IActionResult> UpdateWithJwt(string id, [FromBody] UserModel user)
        {
            string timestamp = _dateTimeService.GetDateTimeNow();
            string method = "POST";
            string queryString = "api/user/upd-with-jwt/{id}";

            if (string.IsNullOrEmpty(id) || user is null)
            {
                return BadRequest($"[{timestamp}] Переданные параметры оказались пустыми или равными null");
            }
            try
            {
                await _userService.UpdateAsync(id, user);

                string token = _tokenService.GenerateJwtToken(user);
                string refreshToken = _tokenService.GenerateRefreshToken();
                Response.Headers.Add("Authorization", $"Bearer {token}");
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
                    JwtToken = token,
                    RefreshToken = refreshToken
                };

                _logger.LogInformation($"INFO: [{timestamp}] User with id - [{id}] successfully updated with jwt-token. Method - [{method}]. Query string - [{queryString}]");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] An error occurred: {ex.Message}. Method - [{method}]. Query string - [{queryString}]");
                return StatusCode(500, $"[{timestamp}] Произошла внутрянняя ошибка сервера. Подробности: {ex.Message}");
            }
        }

        // Проверка почты на уникальность
        //POST: api/user/check-email

        [HttpPost]
        [Route("check-email")]
        public async Task<IActionResult> CheckEmailAvailabilityAsync([FromBody] string email)
        {
            string timestamp = _dateTimeService.GetDateTimeNow();
            string method = "POST";
            string queryString = "api/user/check-email";

            if (string.IsNullOrEmpty(email))
            {
                _logger.LogError($"ERROR: [{timestamp}] Parameters email is null or empty. Method - [{method}]. Query string - [{queryString}]");
                return BadRequest($"[{timestamp}] Переданные параметры оказались пустыми или равными null");
            }
            try
            {
                bool isAvailable = await _userService.CheckEmailAvailabilityAsync(email);
                _logger.LogInformation($"INFO: [{timestamp}] Email - [{email}] successfully checked. Result - [{isAvailable}]. Method - [{method}]. Query string - [{queryString}]");
                return Ok(isAvailable);
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] An error occurred: {ex.Message}. Method - [{method}]. Query string - [{queryString}]");
                return StatusCode(500, $"[{timestamp}] Произошла внутрянняя ошибка сервера. Подробности: {ex.Message}");
            }
        }
        // Проверка логина на уникальность
        //POST: api/user/check-login

        [HttpPost]
        [Route("check-login")]
        public async Task<IActionResult> CheckLoginAvailabilityAsync([FromBody] string login)
        {
            string timestamp = _dateTimeService.GetDateTimeNow();
            string method = "POST";
            string queryString = "api/user/check-login";

            if (string.IsNullOrEmpty(login))
            {
                _logger.LogError($"ERROR: [{timestamp}] Parameters login is null or empty. Method - [{method}]. Query string - [{queryString}]");
                return BadRequest($"[{timestamp}] Переданные параметры оказались пустыми или равными null");
            }
            try
            {
                bool isAvailable = await _userService.CheckLoginAvailabilityAsync(login);
                _logger.LogInformation($"INFO: [{timestamp}] Login - [{login}] successfully checked. Result - [{isAvailable}]. Method - [{method}]. Query string - [{queryString}]");
                return Ok(isAvailable);
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] An error occurred: {ex.Message}. Method - [{method}]. Query string - [{queryString}]");
                return StatusCode(500, $"[{timestamp}] Произошла внутрянняя ошибка сервера. Подробности: {ex.Message}");
            }
        }
        // Получить пользователей по логину
        // GET: api/user/get/{login}

        [Authorize]
        [HttpGet]
        [Route("get/{login}")]
        public async Task<IActionResult> FindByLoginAsync(string login)
        {
            string timestamp = _dateTimeService.GetDateTimeNow();
            string method = "GET";
            string queryString = "api/user/get/{login}";

            if (string.IsNullOrEmpty(login))
            {
                _logger.LogError($"ERROR: [{timestamp}] Parameters login is null or empty. Method - [{method}]. Query string - [{queryString}]");
                return BadRequest($"[{timestamp}] Переданные параметры оказались пустыми или равными null");
            }
            try
            {
                List<UserModel> users = await _userService.FindByLoginAsync(login);
                _logger.LogInformation($"INFO: [{timestamp}] Received {users.Count} users from db with matching login - [{login}]. Method - [{method}]. Query string - [{queryString}]");
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] An error occurred: {ex.Message}. Method - [{method}]. Query string - [{queryString}]");
                return StatusCode(500, $"[{timestamp}] Произошла внутрянняя ошибка сервера. Подробности: {ex.Message}");
            }

        }


    }
}
