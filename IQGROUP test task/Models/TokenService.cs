using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace IQGROUP_test_task.Models
{
    public class TokenService : ITokenService
    {
        IUserService _userService;
        IConfiguration _config;

        public TokenService(IUserService userService, IConfiguration config)
        {
            _userService = userService;
            _config = config;
        }
        // Генерация jwt-токена

        public string GenerateJwtToken(UserModel user)
        {
            try
            {
                string role = "default";
                if (user.Email == "admin" && user.Password == "admin")
                    role = "admin";
                List<Claim> claims = new List<Claim>
                {
                    new Claim("_id", user._id),
                    new Claim("Email", user.Email),
                    new Claim(ClaimTypes.Role, role)
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
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                // log
                return null;
            }
        }

        // Генерация Refresh токна
        // Получается токен вида: RA2Isc+d/w51Y1vttEk2/rx1DUuOi7CLCvHu41rjbpI=
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        // Обновить jwt-токен

        public async Task<string> UpdateJwtToken(string token)
        {
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
                if (user != null)
                {
                    //log
                    return null;
                }
                return null;

            }
            catch (Exception ex)
            {
                return null;
            }
           

        }


        // Проверить истёкший jwt-token
        private ClaimsPrincipal GetPrincipalExpiredToken(string expiredToken)
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

            if (jwtSecureToken == null || !jwtSecureToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
            {
                // Если равен null - jwt-token is invalid
                // Логика обработки данного момента
                return null;
            }
            return principal;
        }
    }
}
