namespace IQGROUP_test_task.Models
{
    public class AuthorizationService : IAuthorizationService
    {
        IHttpContextAccessor _httpContextAccessor;
        public AuthorizationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetAuthorizetionHeader()
        {
            _httpContextAccessor.HttpContext.Response.Headers.TryGetValue("Authorization", out var resValues);
            _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var reqValues);
            return reqValues.ToString() + resValues.ToString();
        }
    }
}
