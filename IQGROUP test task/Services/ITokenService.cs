using IQGROUP_test_task.Models;

namespace IQGROUP_test_task.Services
{
    public interface ITokenService
    {
        public string GenerateJwtToken(UserModel user);
        public string GenerateRefreshToken();
        public Task<string> UpdateJwtTokenAsync(string token);
    }
}
