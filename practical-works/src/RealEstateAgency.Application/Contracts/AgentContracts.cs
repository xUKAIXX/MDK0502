using System.ComponentModel.DataAnnotations;

namespace RealEstateAgency.Application.Contracts;

public sealed record AgentDto(
    int Id,
    string FullName,
    string Phone,
    string? Email,
    int ExperienceYears,
    int PropertiesCount);

public sealed record CreateAgentRequest(
    [Required(ErrorMessage = "Введите ФИО агента")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "ФИО должно быть от 2 до 100 символов")]
    string FullName,
    [Required(ErrorMessage = "Введите телефон агента")]
    [StringLength(30, MinimumLength = 5, ErrorMessage = "Телефон должен быть от 5 до 30 символов")]
    string Phone,
    [EmailAddress(ErrorMessage = "Введите корректный email")]
    string? Email,
    [Range(0, 80, ErrorMessage = "Стаж должен быть от 0 до 80 лет")]
    int ExperienceYears);

public sealed record UpdateAgentRequest(
    [Required(ErrorMessage = "Введите ФИО агента")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "ФИО должно быть от 2 до 100 символов")]
    string FullName,
    [Required(ErrorMessage = "Введите телефон агента")]
    [StringLength(30, MinimumLength = 5, ErrorMessage = "Телефон должен быть от 5 до 30 символов")]
    string Phone,
    [EmailAddress(ErrorMessage = "Введите корректный email")]
    string? Email,
    [Range(0, 80, ErrorMessage = "Стаж должен быть от 0 до 80 лет")]
    int ExperienceYears);
