using System.ComponentModel.DataAnnotations;

namespace DocStore.Models
{
    /// <summary>
    /// Класс для описания сущности пользователя
    /// </summary>
    public class User
    {
        public virtual int Id { get; set; }
        [Required( ErrorMessage = "Введите логин")]
        public virtual string Name { get; set; }
        [Required (ErrorMessage = "Введите пароль")]
        public virtual string Password { get; set; }
        public virtual Role Role { get; set; }
    }
}