using RealEstateAgency.Domain;
using Xunit;

namespace RealEstateAgency.Tests.Domain;

public sealed class PropertyTests
{
    [Fact]
    public void Create_WithValidData_SetsForSaleStatus()
    {
        var property = CreateProperty();

        Assert.Equal(PropertyStatus.ForSale, property.Status);
        Assert.Equal("Тестовая квартира", property.Title);
        Assert.Equal(10_000_000m, property.Price);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidPrice_Throws(decimal price)
    {
        Assert.Throws<DomainException>(() => CreateProperty(price: price));
    }

    [Fact]
    public void Create_WhenFloorGreaterThanTotalFloors_Throws()
    {
        Assert.Throws<DomainException>(() => CreateProperty(floor: 10, totalFloors: 5));
    }

    [Fact]
    public void ChangeStatus_FromForSaleToSold_Succeeds()
    {
        var property = CreateProperty();

        property.ChangeStatus(PropertyStatus.Sold);

        Assert.Equal(PropertyStatus.Sold, property.Status);
    }

    [Fact]
    public void ChangeStatus_FromForSaleToRented_Throws()
    {
        var property = CreateProperty();

        Assert.Throws<DomainException>(() => property.ChangeStatus(PropertyStatus.Rented));
    }

    [Fact]
    public void UpdateDetails_AfterSold_Throws()
    {
        var property = CreateProperty();
        property.ChangeStatus(PropertyStatus.Sold);

        Assert.Throws<DomainException>(() => property.UpdateDetails(
            "Новое название",
            "Новый адрес, 1",
            12_000_000m,
            65,
            2,
            3,
            9,
            2015,
            null,
            CreatePropertyType(),
            CreateDistrict(),
            CreateAgent()));
    }

    private static Property CreateProperty(
        decimal price = 10_000_000m,
        int floor = 3,
        int totalFloors = 9,
        int yearBuilt = 2015)
    {
        return Property.Create(
            "Тестовая квартира",
            "Невский проспект, 100",
            price,
            60,
            2,
            floor,
            totalFloors,
            yearBuilt,
            "Описание тестового объекта",
            CreatePropertyType(),
            CreateDistrict(),
            CreateAgent(),
            null,
            null,
            DateTime.UtcNow);
    }

    private static PropertyType CreatePropertyType() => PropertyType.Create("Квартира");

    private static District CreateDistrict() => District.Create("Невский", "Санкт-Петербург");

    private static Agent CreateAgent() => Agent.Create(
        "Иван Иванов",
        "+7 (900) 111-11-11",
        "ivanov@agency.local",
        5);
}
