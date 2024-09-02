using IQGROUP_test_task.Models;
using Microsoft.AspNetCore.Mvc;

namespace IQGROUP_test_task.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : Controller
    {
        IUserService _userService;
        ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }
        // GET: api/users

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
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
        // GET: api/users/{id}

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
        // POST: api/users/auth

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
                    return Ok(user);
                }
            }
        }
        // POST: api/users/reg

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
                    return Ok(user.Login);
                    // log
                }
                catch (Exception ex)
                {
                    // log
                    return Ok(ex.Message);
                }
            }
        }
        //POST: api/users/rmv/{id

        [HttpPost]
        [Route("rmv/{id}")]
        public async Task<IActionResult> RemoveAsync(string id)
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
        //POST: api/users/upd

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
