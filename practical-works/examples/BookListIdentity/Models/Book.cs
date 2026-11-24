using System.ComponentModel.DataAnnotations;

namespace BookList.Models;

public class Book
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название книги")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Название от 2 до 100 символов")]
    [Display(Name = "Название")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите автора")]
    [StringLength(60, MinimumLength = 2)]
    [Display(Name = "Автор")]
    public string Author { get; set; } = string.Empty;

    [Range(1500, 2100, ErrorMessage = "Год должен быть от 1500 до 2100")]
    [Display(Name = "Год издания")]
    public int Year { get; set; }

    [Required(ErrorMessage = "Выберите жанр")]
    [Display(Name = "Жанр")]
    public string Genre { get; set; } = string.Empty;

    [Display(Name = "Владелец")]
    public string? OwnerId { get; set; }

    [Display(Name = "Владелец")]
    public ApplicationUser? Owner { get; set; }
}
