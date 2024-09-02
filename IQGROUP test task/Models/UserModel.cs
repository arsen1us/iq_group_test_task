using System.ComponentModel.DataAnnotations;

namespace IQGROUP_test_task.Models
{
    public class UserModel
    {
        public UserModel(string id, string login, string email, string password)
        {
            _id = id;
            Login = login;
            Email = email;
            Password = password;
        }
        public string _id { get; set; } = null!;

        public string Login { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
