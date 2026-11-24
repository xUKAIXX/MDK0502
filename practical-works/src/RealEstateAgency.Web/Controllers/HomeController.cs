using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateAgency.Application.Deals;
using RealEstateAgency.Application.Properties;
using RealEstateAgency.Web.ViewModels;

namespace RealEstateAgency.Web.Controllers;

public sealed class HomeController(
    IPropertyService properties,
    IDealService deals) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var result = await properties.GetPagedAsync(
            new PropertyFilter { Page = 1, PageSize = 6 },
            cancellationToken);

        var allDeals = await deals.GetAllAsync(cancellationToken);

        var model = new HomeViewModel
        {
            TotalProperties = result.Total,
            TotalDeals = allDeals.Count,
            RecentProperties = result.Items
        };

        return View(model);
    }

    [Authorize(Policy = "OnlyAdults")]
    public IActionResult Adults()
    {
        return View();
    }

    [AllowAnonymous]
    public IActionResult Privacy()
    {
        return View();
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
