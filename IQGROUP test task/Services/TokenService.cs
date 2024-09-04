using IQGROUP_test_task.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace IQGROUP_test_task.Services
{
    public class TokenService : ITokenService
    {
        IUserService _userService;
        IConfiguration _config;
        ILogger<TokenService> _logger;
        IDateTimeService _dateTimeService;

        public TokenService(IUserService userService, IConfiguration config, ILogger<TokenService> logger, IDateTimeService dateTimeService)
        {
            _userService = userService;
            _config = config;
            _logger = logger;
            _dateTimeService = dateTimeService;
        }
        // Генерация jwt-токена

        public string GenerateJwtToken(UserModel user)
        {
            string timestamp = _dateTimeService.GetDateTimeNow();
            try
            {
                List<Claim> claims = new List<Claim>
                {
                    new Claim("_id", user._id),
                    new Claim("Email", user.Email)
                };

                JwtSecurityToken token = new JwtSecurityToken
                (
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromHours(1)),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"])), SecurityAlgorithms.HmacSha256Signature)
                );

                var tokenHandler = new JwtSecurityTokenHandler();

                _logger.LogInformation($"INFO: [{timestamp}] Jwt-token successfully generated. User id - [{user._id}], email - [{user.Email}]");
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] Generating jwt-token error. User id - [{user._id}], email - [{user.Email}]");
                throw new Exception($"Unexpected error. Details: {ex.Message}");
            }
        }

        // Генерация Refresh токна
        // Получается токен вида: RA2Isc+d/w51Y1vttEk2/rx1DUuOi7CLCvHu41rjbpI=
        public string GenerateRefreshToken()
        {
            string timestamp = _dateTimeService.GetDateTimeNow();
            var randomNumber = new byte[32];
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(randomNumber);

                _logger.LogInformation($"INFO: [{timestamp}] Refresh-token successfully generated");
                return Convert.ToBase64String(randomNumber);
            }
        }
        // Обновить jwt-токен

        public async Task<string> UpdateJwtTokenAsync(string token)
        {
            string timestamp = _dateTimeService.GetDateTimeNow();
            try
            {
                var principal = GetPrincipalExpiredToken(token);
                if (principal != null)
                {
                    // log ошибка счмтывания jwt-токен или он поддельный 
                    return null;
                }

                string _id = principal.FindFirst("_id").Value;
                string email = principal.FindFirst("Email").Value;

                if (_id is null || email is null)
                {
                    // log ошибка счмтывания jwt-токен или он поддельный 
                    return null;
                }

                var user = await _userService.FindByIdAsync(_id);

                _logger.LogInformation($"INFO: [{timestamp}] Refresh-token successfully updated");
                return GenerateJwtToken(user);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: [{timestamp}] Jwt-token update with unexpected error. Details: {ex.Message}");
                throw new Exception($"Unexpected error. Details: {ex.Message}");
            }


        }


        // Проверить истёкший jwt-token
        private ClaimsPrincipal GetPrincipalExpiredToken(string expiredToken)
        {
            string timestamp = _dateTimeService.GetDateTimeNow();
            try
            {
                var tokenValidationParameter = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]))
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(expiredToken, tokenValidationParameter, out SecurityToken securityToken);

                var jwtSecureToken = securityToken as JwtSecurityToken;

                if (jwtSecureToken != null || jwtSecureToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
                    return principal;

                var claims = new ClaimsIdentity();

                _logger.LogInformation($"INFO: [{timestamp}] Successfully received principal expired token");
                return new ClaimsPrincipal(claims);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: [{timestamp}] Get principal expired token unexpected error. Details: {ex.Message}");
                throw new Exception($"Unexpected error. Details: {ex.Message}");
            }

        }
    }
}
