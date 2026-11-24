using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstateAgency.Application.Contracts;
using RealEstateAgency.Application.Dictionaries;
using RealEstateAgency.Application.Properties;
using RealEstateAgency.Domain;
using RealEstateAgency.Web.ViewModels;

namespace RealEstateAgency.Web.Controllers;

[Authorize]
public sealed class PropertiesController(
    IPropertyService properties,
    IDictionaryService dictionaries) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> Index(
        string? search,
        string? status,
        int? propertyTypeId,
        int? districtId,
        decimal? minPrice,
        decimal? maxPrice,
        string? sortBy,
        string? sortOrder,
        int page = 1,
        int pageSize = 6,
        CancellationToken cancellationToken = default)
    {
        var filter = new PropertyFilter
        {
            Search = search,
            Status = status,
            PropertyTypeId = propertyTypeId,
            DistrictId = districtId,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            SortBy = sortBy,
            SortOrder = sortOrder,
            Page = page,
            PageSize = pageSize
        };

        if (User.Identity?.IsAuthenticated == true && !User.IsInRole("Admin") && !User.IsInRole("Agent"))
        {
            filter = filter with { OwnerId = User.FindFirstValue(ClaimTypes.NameIdentifier) };
        }

        var result = await properties.GetPagedAsync(filter, cancellationToken);
        var dictionary = await dictionaries.GetDictionariesAsync(cancellationToken);

        var model = new PropertyIndexViewModel
        {
            Items = result.Items,
            Total = result.Total,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages,
            Search = search,
            Status = status,
            PropertyTypeId = propertyTypeId,
            DistrictId = districtId,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            SortBy = sortBy,
            SortOrder = sortOrder,
            PropertyTypes = dictionary.PropertyTypes,
            Districts = dictionary.Districts
        };

        return View(model);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var property = await properties.GetByIdAsync(id, cancellationToken);

        if (property is null)
        {
            return NotFound();
        }

        return View(property);
    }

    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = new PropertyFormViewModel();
        await FillDictionariesAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> Create(PropertyFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await FillDictionariesAsync(model, cancellationToken);
            return View(model);
        }

        var request = new CreatePropertyRequest(
            model.Title,
            model.Address,
            model.Price,
            model.Area,
            model.Rooms,
            model.Floor,
            model.TotalFloors,
            model.YearBuilt,
            model.Description,
            model.PropertyTypeId,
            model.DistrictId,
            model.AgentId);

        try
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ownerName = User.Identity?.Name;

            await properties.CreateAsync(request, ownerId, ownerName, cancellationToken);

            TempData["Success"] = "Объект недвижимости успешно добавлен.";
            return RedirectToAction(nameof(Index));
        }
        catch (DomainException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await FillDictionariesAsync(model, cancellationToken);
            return View(model);
        }
    }

    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var property = await properties.GetByIdAsync(id, cancellationToken);

        if (property is null)
        {
            return NotFound();
        }

        var model = new PropertyFormViewModel
        {
            Id = property.Id,
            Title = property.Title,
            Address = property.Address,
            Price = property.Price,
            Area = property.Area,
            Rooms = property.Rooms,
            Floor = property.Floor,
            TotalFloors = property.TotalFloors,
            YearBuilt = property.YearBuilt,
            Description = property.Description,
            PropertyTypeId = property.PropertyTypeId,
            DistrictId = property.DistrictId,
            AgentId = property.AgentId
        };

        await FillDictionariesAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> Edit(PropertyFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await FillDictionariesAsync(model, cancellationToken);
            return View(model);
        }

        var request = new UpdatePropertyRequest(
            model.Title,
            model.Address,
            model.Price,
            model.Area,
            model.Rooms,
            model.Floor,
            model.TotalFloors,
            model.YearBuilt,
            model.Description,
            model.PropertyTypeId,
            model.DistrictId,
            model.AgentId);

        try
        {
            var updated = await properties.UpdateAsync(model.Id, request, cancellationToken);

            if (updated is null)
            {
                return NotFound();
            }

            TempData["Success"] = "Объект недвижимости обновлён.";
            return RedirectToAction(nameof(Index));
        }
        catch (DomainException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await FillDictionariesAsync(model, cancellationToken);
            return View(model);
        }
    }

    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var property = await properties.GetByIdAsync(id, cancellationToken);

        if (property is null)
        {
            return NotFound();
        }

        return View(property);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await properties.DeleteAsync(id, cancellationToken);

            if (!deleted)
            {
                return NotFound();
            }

            TempData["Success"] = "Объект недвижимости удалён.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "Нельзя удалить объект: с ним связаны сделки.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> ChangeStatus(int id, int status, CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(typeof(PropertyStatus), status))
        {
            TempData["Error"] = "Неизвестный статус объекта.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var updated = await properties.ChangeStatusAsync(id, (PropertyStatus)status, cancellationToken);

            if (!updated)
            {
                return NotFound();
            }

            TempData["Success"] = "Статус объекта обновлён.";
        }
        catch (DomainException exception)
        {
            TempData["Error"] = exception.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task FillDictionariesAsync(PropertyFormViewModel model, CancellationToken cancellationToken)
    {
        var dictionary = await dictionaries.GetDictionariesAsync(cancellationToken);
        model.PropertyTypes = dictionary.PropertyTypes;
        model.Districts = dictionary.Districts;
        model.Agents = dictionary.Agents;
    }
}
