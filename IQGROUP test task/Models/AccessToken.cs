namespace IQGROUP_test_task.Models
{
    public class AccessToken
    {
        private string _jwtToken;
        private string _refreshToken;

        public AccessToken(string jwtToken, string refreshToken)
        {
            _jwtToken = jwtToken;
            _refreshToken = refreshToken;
        }

        public string JwtToken
        {
            get { return _jwtToken; }
            set { _jwtToken = value; }
        }

        public string RefreshToken
        {
            get { return _refreshToken; }
            set { _refreshToken = value; }
        }
    }
}
