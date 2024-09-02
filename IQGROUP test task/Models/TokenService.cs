namespace IQGROUP_test_task.Models
{
    public class TokenService : ITokenService
    {
        IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }
        public string GenerateJwtToken(UserModel user)
        {

        }

        public string GenerateRefreshToken()
        {

        }
    }
}
