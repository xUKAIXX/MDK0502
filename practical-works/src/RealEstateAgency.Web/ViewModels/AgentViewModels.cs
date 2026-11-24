using System.ComponentModel.DataAnnotations;

namespace RealEstateAgency.Web.ViewModels;

public sealed class AgentFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите ФИО агента")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "ФИО должно быть от 2 до 100 символов")]
    [Display(Name = "ФИО")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите телефон агента")]
    [StringLength(30, MinimumLength = 5, ErrorMessage = "Телефон должен быть от 5 до 30 символов")]
    [Display(Name = "Телефон")]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Введите корректный email")]
    [StringLength(100, ErrorMessage = "Email не должен превышать 100 символов")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Range(0, 80, ErrorMessage = "Стаж должен быть от 0 до 80 лет")]
    [Display(Name = "Стаж, лет")]
    public int ExperienceYears { get; set; }
}
