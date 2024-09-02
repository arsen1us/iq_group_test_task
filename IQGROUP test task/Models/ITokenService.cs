namespace IQGROUP_test_task.Models
{
    public interface ITokenService
    {
        public string GenerateJwtToken(UserModel user);
        public string GenerateRefreshToken();
    }
}
