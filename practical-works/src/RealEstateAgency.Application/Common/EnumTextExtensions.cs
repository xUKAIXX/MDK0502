using RealEstateAgency.Domain;

namespace RealEstateAgency.Application.Common;

public static class EnumTextExtensions
{
    public static string ToRussian(this PropertyStatus status) => status switch
    {
        PropertyStatus.ForSale => "Продаётся",
        PropertyStatus.ForRent => "Сдаётся",
        PropertyStatus.Sold => "Продано",
        PropertyStatus.Rented => "Сдано",
        _ => status.ToString()
    };

    public static string ToRussian(this DealType type) => type switch
    {
        DealType.Purchase => "Покупка",
        DealType.Rent => "Аренда",
        _ => type.ToString()
    };

    public static string ToRussian(this DealStatus status) => status switch
    {
        DealStatus.New => "Новая",
        DealStatus.InProgress => "В работе",
        DealStatus.Completed => "Завершена",
        DealStatus.Cancelled => "Отменена",
        _ => status.ToString()
    };
}
