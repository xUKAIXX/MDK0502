using System.ComponentModel.DataAnnotations;

namespace RealEstateAgency.Application.Contracts;

public sealed record PropertyDto(
    int Id,
    string Title,
    string Address,
    decimal Price,
    double Area,
    int Rooms,
    int Floor,
    int TotalFloors,
    int YearBuilt,
    string? Description,
    string Status,
    int PropertyTypeId,
    string PropertyType,
    int DistrictId,
    string District,
    int AgentId,
    string Agent,
    string? OwnerName,
    DateTime CreatedAt);

public sealed record PropertyListItemDto(
    int Id,
    string Title,
    string Address,
    decimal Price,
    double Area,
    int Rooms,
    string Status,
    string PropertyType,
    string District);

public sealed record CreatePropertyRequest(
    [Required(ErrorMessage = "Введите название объекта")]
    [StringLength(150, MinimumLength = 5, ErrorMessage = "Название должно быть от 5 до 150 символов")]
    string Title,
    [Required(ErrorMessage = "Введите адрес объекта")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "Адрес должен быть от 5 до 200 символов")]
    string Address,
    [Range(0.01, 1_000_000_000, ErrorMessage = "Цена должна быть в диапазоне от 0,01 до 1 000 000 000")]
    decimal Price,
    [Range(1, 100_000, ErrorMessage = "Площадь должна быть от 1 до 100 000 м²")]
    double Area,
    [Range(0, 50, ErrorMessage = "Количество комнат должно быть от 0 до 50")]
    int Rooms,
    [Range(0, 200, ErrorMessage = "Этаж должен быть от 0 до 200")]
    int Floor,
    [Range(1, 300, ErrorMessage = "Всего этажей должно быть от 1 до 300")]
    int TotalFloors,
    [Range(1500, 2100, ErrorMessage = "Год постройки должен быть от 1500 до 2100")]
    int YearBuilt,
    string? Description,
    [Range(1, int.MaxValue, ErrorMessage = "Выберите тип объекта")]
    int PropertyTypeId,
    [Range(1, int.MaxValue, ErrorMessage = "Выберите район")]
    int DistrictId,
    [Range(1, int.MaxValue, ErrorMessage = "Выберите агента")]
    int AgentId);

public sealed record UpdatePropertyRequest(
    [Required(ErrorMessage = "Введите название объекта")]
    [StringLength(150, MinimumLength = 5, ErrorMessage = "Название должно быть от 5 до 150 символов")]
    string Title,
    [Required(ErrorMessage = "Введите адрес объекта")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "Адрес должен быть от 5 до 200 символов")]
    string Address,
    [Range(0.01, 1_000_000_000, ErrorMessage = "Цена должна быть в диапазоне от 0,01 до 1 000 000 000")]
    decimal Price,
    [Range(1, 100_000, ErrorMessage = "Площадь должна быть от 1 до 100 000 м²")]
    double Area,
    [Range(0, 50, ErrorMessage = "Количество комнат должно быть от 0 до 50")]
    int Rooms,
    [Range(0, 200, ErrorMessage = "Этаж должен быть от 0 до 200")]
    int Floor,
    [Range(1, 300, ErrorMessage = "Всего этажей должно быть от 1 до 300")]
    int TotalFloors,
    [Range(1500, 2100, ErrorMessage = "Год постройки должен быть от 1500 до 2100")]
    int YearBuilt,
    string? Description,
    [Range(1, int.MaxValue, ErrorMessage = "Выберите тип объекта")]
    int PropertyTypeId,
    [Range(1, int.MaxValue, ErrorMessage = "Выберите район")]
    int DistrictId,
    [Range(1, int.MaxValue, ErrorMessage = "Выберите агента")]
    int AgentId);
