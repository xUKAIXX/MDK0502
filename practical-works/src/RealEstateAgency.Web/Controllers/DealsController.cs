using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateAgency.Application.Contracts;
using RealEstateAgency.Application.Deals;
using RealEstateAgency.Application.Properties;
using RealEstateAgency.Domain;
using RealEstateAgency.Web.ViewModels;

namespace RealEstateAgency.Web.Controllers;

[Authorize]
public sealed class DealsController(
    IDealService deals,
    IPropertyService properties) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var items = await deals.GetAllAsync(cancellationToken);
        return View(new DealIndexViewModel { Items = items });
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = new DealFormViewModel();
        await FillAvailablePropertiesAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DealFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await FillAvailablePropertiesAsync(model, cancellationToken);
            return View(model);
        }

        var request = new CreateDealRequest(
            model.PropertyId,
            model.ClientName,
            model.ClientPhone,
            model.Type,
            model.Amount,
            model.Comment);

        try
        {
            await deals.CreateAsync(request, cancellationToken);
            TempData["Success"] = "Сделка успешно создана.";
            return RedirectToAction(nameof(Index));
        }
        catch (DomainException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await FillAvailablePropertiesAsync(model, cancellationToken);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> Start(int id, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await deals.StartAsync(id, cancellationToken);
            TempData[updated ? "Success" : "Error"] = updated
                ? "Сделка взята в работу."
                : "Сделка не найдена.";
        }
        catch (DomainException exception)
        {
            TempData["Error"] = exception.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> Complete(int id, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await deals.CompleteAsync(id, cancellationToken);
            TempData[updated ? "Success" : "Error"] = updated
                ? "Сделка завершена, статус объекта обновлён."
                : "Сделка не найдена.";
        }
        catch (DomainException exception)
        {
            TempData["Error"] = exception.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await deals.CancelAsync(id, cancellationToken);
            TempData[updated ? "Success" : "Error"] = updated
                ? "Сделка отменена."
                : "Сделка не найдена.";
        }
        catch (DomainException exception)
        {
            TempData["Error"] = exception.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task FillAvailablePropertiesAsync(DealFormViewModel model, CancellationToken cancellationToken)
    {
        var result = await properties.GetPagedAsync(
            new PropertyFilter { Page = 1, PageSize = 100 },
            cancellationToken);

        model.AvailableProperties = result.Items
            .Where(property => property.Status is "Продаётся" or "Сдаётся")
            .ToList();
    }
}
