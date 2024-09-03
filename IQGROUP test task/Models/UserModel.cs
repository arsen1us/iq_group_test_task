using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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

        public UserModel() {}

        [JsonPropertyName("_id")]
        public string _id { get; set; } = null!;

        public string Login { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
