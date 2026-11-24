using System.ComponentModel.DataAnnotations;
using RealEstateAgency.Application.Contracts;

namespace RealEstateAgency.Web.ViewModels;

public sealed class DealIndexViewModel
{
    public IReadOnlyList<DealDto> Items { get; set; } = Array.Empty<DealDto>();
}

public sealed class DealFormViewModel
{
    [Required(ErrorMessage = "Выберите объект недвижимости")]
    [Range(1, int.MaxValue, ErrorMessage = "Выберите объект недвижимости")]
    [Display(Name = "Объект недвижимости")]
    public int PropertyId { get; set; }

    [Required(ErrorMessage = "Введите имя клиента")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Имя клиента должно быть от 2 до 100 символов")]
    [Display(Name = "Имя клиента")]
    public string ClientName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите телефон клиента")]
    [StringLength(30, MinimumLength = 5, ErrorMessage = "Телефон клиента должен быть от 5 до 30 символов")]
    [Display(Name = "Телефон клиента")]
    public string ClientPhone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Выберите тип сделки")]
    [Range(1, 2, ErrorMessage = "Выберите тип сделки")]
    [Display(Name = "Тип сделки")]
    public int Type { get; set; } = 1;

    [Required(ErrorMessage = "Введите сумму сделки")]
    [Range(0.01, 1_000_000_000, ErrorMessage = "Сумма сделки должна быть положительной")]
    [Display(Name = "Сумма, ₽")]
    public decimal Amount { get; set; }

    [StringLength(1000, ErrorMessage = "Комментарий не должен превышать 1000 символов")]
    [Display(Name = "Комментарий")]
    public string? Comment { get; set; }

    public IReadOnlyList<PropertyListItemDto> AvailableProperties { get; set; } = Array.Empty<PropertyListItemDto>();
}
