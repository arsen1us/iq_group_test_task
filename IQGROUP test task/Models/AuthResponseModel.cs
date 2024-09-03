namespace IQGROUP_test_task.Models
{
    public class AuthResponseModel
    {
        public string UserId { get; set; }
        public string UserLogin { get; set; }
        public string UserEmail { get; set; }
        public string JwtToken { get; set; }    
        public string RefreshToken { get; set; }
    }
}
