using RealEstateAgency.Domain;
using Xunit;

namespace RealEstateAgency.Tests.Domain;

public sealed class DealTests
{
    [Fact]
    public void Create_PurchaseForSaleProperty_Succeeds()
    {
        var property = CreateProperty();

        var deal = Deal.Create(
            property,
            "Ольга Смирнова",
            "+7 (911) 111-22-33",
            DealType.Purchase,
            12_500_000m,
            DateTime.UtcNow);

        Assert.Equal(DealStatus.New, deal.Status);
        Assert.Equal(DealType.Purchase, deal.Type);
    }

    [Fact]
    public void Create_PurchaseForRentProperty_Throws()
    {
        var property = CreateProperty();
        property.ChangeStatus(PropertyStatus.ForRent);

        Assert.Throws<DomainException>(() => Deal.Create(
            property,
            "Ольга Смирнова",
            "+7 (911) 111-22-33",
            DealType.Purchase,
            12_500_000m,
            DateTime.UtcNow));
    }

    [Fact]
    public void Create_RentForSaleProperty_Throws()
    {
        var property = CreateProperty();

        Assert.Throws<DomainException>(() => Deal.Create(
            property,
            "Ольга Смирнова",
            "+7 (911) 111-22-33",
            DealType.Rent,
            100_000m,
            DateTime.UtcNow));
    }

    [Fact]
    public void Complete_AfterCancel_Throws()
    {
        var property = CreateProperty();
        var deal = Deal.Create(
            property,
            "Ольга Смирнова",
            "+7 (911) 111-22-33",
            DealType.Purchase,
            12_500_000m,
            DateTime.UtcNow);

        deal.Cancel();

        Assert.Throws<DomainException>(deal.Complete);
    }

    [Fact]
    public void Cancel_AfterComplete_Throws()
    {
        var property = CreateProperty();
        var deal = Deal.Create(
            property,
            "Ольга Смирнова",
            "+7 (911) 111-22-33",
            DealType.Purchase,
            12_500_000m,
            DateTime.UtcNow);

        deal.Complete();

        Assert.Throws<DomainException>(deal.Cancel);
    }

    private static Property CreateProperty()
    {
        return Property.Create(
            "Тестовая квартира",
            "Невский проспект, 100",
            12_500_000m,
            65,
            2,
            5,
            9,
            2012,
            null,
            PropertyType.Create("Квартира"),
            District.Create("Невский", "Санкт-Петербург"),
            Agent.Create("Иван Иванов", "+7 (900) 111-11-11", "ivanov@agency.local", 5),
            null,
            null,
            DateTime.UtcNow);
    }
}
