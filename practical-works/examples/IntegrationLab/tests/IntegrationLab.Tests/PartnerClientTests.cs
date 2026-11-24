using System.Net;
using IntegrationClient;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace IntegrationLab.Tests;

public sealed class PartnerClientTests
{
    [Fact]
    public async Task CreateOrderAsync_WhenPartnerReturns201_ReturnsOrder()
    {
        var handler = new FakePartnerHandler();
        var client = CreateClient(handler);

        var order = await client.CreateOrderAsync(
            new OrderRequest("customer-1", 1500m, "RUB"),
            Guid.NewGuid().ToString("N"),
            CancellationToken.None);

        Assert.NotNull(order);
        Assert.Equal("Created", order!.Status);
        Assert.Equal(1, handler.Requests);
    }

    [Fact]
    public async Task CreateOrderAsync_WhenSameKeyIsReused_ReturnsSameOrder()
    {
        var handler = new FakePartnerHandler();
        var client = CreateClient(handler);
        var key = Guid.NewGuid().ToString("N");
        var request = new OrderRequest("customer-1", 1500m, "RUB");

        var first = await client.CreateOrderAsync(request, key, CancellationToken.None);
        var second = await client.CreateOrderAsync(request, key, CancellationToken.None);

        Assert.NotNull(first);
        Assert.Equal(first!.Id, second!.Id);
        Assert.Equal(2, handler.Requests);
    }

    [Fact]
    public async Task GetOrderAsync_WhenPartnerReturns404_ReturnsNull()
    {
        var client = CreateClient(new FakePartnerHandler());

        var order = await client.GetOrderAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(order);
    }

    private static PartnerClient CreateClient(HttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5100")
        };

        return new PartnerClient(httpClient, NullLogger<PartnerClient>.Instance);
    }
}
