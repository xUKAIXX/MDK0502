using System.ComponentModel.DataAnnotations;

namespace RealEstateAgency.Application.Contracts;

public sealed record DealDto(
    int Id,
    int PropertyId,
    string PropertyTitle,
    string ClientName,
    string ClientPhone,
    string Type,
    string Status,
    decimal Amount,
    string? Comment,
    DateTime CreatedAt);

public sealed record CreateDealRequest(
    [Range(1, int.MaxValue, ErrorMessage = "Выберите объект недвижимости")]
    int PropertyId,
    [Required(ErrorMessage = "Введите имя клиента")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Имя клиента должно быть от 2 до 100 символов")]
    string ClientName,
    [Required(ErrorMessage = "Введите телефон клиента")]
    [StringLength(30, MinimumLength = 5, ErrorMessage = "Телефон клиента должен быть от 5 до 30 символов")]
    string ClientPhone,
    [Range(1, 2, ErrorMessage = "Выберите тип сделки")]
    int Type,
    [Range(0.01, 1_000_000_000, ErrorMessage = "Сумма сделки должна быть положительной")]
    decimal Amount,
    string? Comment);
