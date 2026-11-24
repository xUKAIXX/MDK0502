using System.ComponentModel.DataAnnotations;
using RealEstateAgency.Application.Contracts;

namespace RealEstateAgency.Web.ViewModels;

public sealed class PropertyIndexViewModel
{
    public IReadOnlyList<PropertyListItemDto> Items { get; set; } = Array.Empty<PropertyListItemDto>();

    public int Total { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 6;

    public int TotalPages { get; set; } = 1;

    public string? Search { get; set; }

    public string? Status { get; set; }

    public int? PropertyTypeId { get; set; }

    public int? DistrictId { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public string? SortBy { get; set; }

    public string? SortOrder { get; set; }

    public IReadOnlyList<PropertyTypeDto> PropertyTypes { get; set; } = Array.Empty<PropertyTypeDto>();

    public IReadOnlyList<DistrictDto> Districts { get; set; } = Array.Empty<DistrictDto>();
}

public sealed class PropertyFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название объекта")]
    [StringLength(150, MinimumLength = 5, ErrorMessage = "Название должно быть от 5 до 150 символов")]
    [Display(Name = "Название")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите адрес объекта")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "Адрес должен быть от 5 до 200 символов")]
    [Display(Name = "Адрес")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите цену")]
    [Range(0.01, 1_000_000_000, ErrorMessage = "Цена должна быть в диапазоне от 0,01 до 1 000 000 000")]
    [Display(Name = "Цена, ₽")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Введите площадь")]
    [Range(1, 100_000, ErrorMessage = "Площадь должна быть от 1 до 100 000 м²")]
    [Display(Name = "Площадь, м²")]
    public double Area { get; set; }

    [Required(ErrorMessage = "Укажите количество комнат")]
    [Range(0, 50, ErrorMessage = "Количество комнат должно быть от 0 до 50")]
    [Display(Name = "Комнат")]
    public int Rooms { get; set; }

    [Required(ErrorMessage = "Укажите этаж")]
    [Range(0, 200, ErrorMessage = "Этаж должен быть от 0 до 200")]
    [Display(Name = "Этаж")]
    public int Floor { get; set; }

    [Required(ErrorMessage = "Укажите количество этажей")]
    [Range(1, 300, ErrorMessage = "Количество этажей должно быть от 1 до 300")]
    [Display(Name = "Этажей в доме")]
    public int TotalFloors { get; set; }

    [Required(ErrorMessage = "Укажите год постройки")]
    [Range(1500, 2100, ErrorMessage = "Год постройки должен быть от 1500 до 2100")]
    [Display(Name = "Год постройки")]
    public int YearBuilt { get; set; }

    [StringLength(2000, ErrorMessage = "Описание не должно превышать 2000 символов")]
    [Display(Name = "Описание")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Выберите тип объекта")]
    [Range(1, int.MaxValue, ErrorMessage = "Выберите тип объекта")]
    [Display(Name = "Тип объекта")]
    public int PropertyTypeId { get; set; }

    [Required(ErrorMessage = "Выберите район")]
    [Range(1, int.MaxValue, ErrorMessage = "Выберите район")]
    [Display(Name = "Район")]
    public int DistrictId { get; set; }

    [Required(ErrorMessage = "Выберите агента")]
    [Range(1, int.MaxValue, ErrorMessage = "Выберите агента")]
    [Display(Name = "Агент")]
    public int AgentId { get; set; }

    public IReadOnlyList<PropertyTypeDto> PropertyTypes { get; set; } = Array.Empty<PropertyTypeDto>();

    public IReadOnlyList<DistrictDto> Districts { get; set; } = Array.Empty<DistrictDto>();

    public IReadOnlyList<AgentDto> Agents { get; set; } = Array.Empty<AgentDto>();
}
