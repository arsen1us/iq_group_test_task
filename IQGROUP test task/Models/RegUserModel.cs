using System.ComponentModel.DataAnnotations;

namespace IQGROUP_test_task.Models
{
    public class RegUserModel
    {
        [Required(ErrorMessage = "Это поле должно быть заполнено!")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Это поле должно быть заполнено!")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Адрес введён некорректно")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Это поле должно быть заполнено!")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Пароль слишком короткий!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Это поле должно быть заполнено!")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают!")]
        public string ConfirmPassword { get; set; }
    }
}
