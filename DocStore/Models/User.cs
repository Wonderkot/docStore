using System.ComponentModel.DataAnnotations;

namespace DocStore.Models
{
    /// <summary>
    /// Класс для описания сущности пользователя
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        [Required( ErrorMessage = "Введите логин")]
        public string Name { get; set; }
        [Required (ErrorMessage = "Введите пароль")]
        public string Password { get; set; }
        public Role Role { get; set; }
    }
}