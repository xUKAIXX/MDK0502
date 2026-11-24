using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BookList.Models;

/// <summary>
/// Пользователь приложения. Наследуется от IdentityUser, поэтому уже имеет
/// Id, UserName, Email, PasswordHash и другие поля Identity.
/// </summary>
public class ApplicationUser : IdentityUser
{
    [Display(Name = "Имя")]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Дата регистрации")]
    public DateTime RegisteredAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Навигационное свойство «один ко многим»: книги этого пользователя.
    /// </summary>
    public List<Book> Books { get; set; } = new();
}
