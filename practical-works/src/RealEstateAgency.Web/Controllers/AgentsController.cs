using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstateAgency.Application.Agents;
using RealEstateAgency.Application.Contracts;
using RealEstateAgency.Domain;
using RealEstateAgency.Web.ViewModels;

namespace RealEstateAgency.Web.Controllers;

[Authorize]
public sealed class AgentsController(IAgentService agents) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var items = await agents.GetAllAsync(cancellationToken);
        return View(items);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View(new AgentFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(AgentFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var request = new CreateAgentRequest(
                model.FullName,
                model.Phone,
                model.Email,
                model.ExperienceYears);

            await agents.CreateAsync(request, cancellationToken);
            TempData["Success"] = "Агент добавлен.";
            return RedirectToAction(nameof(Index));
        }
        catch (DomainException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(model);
        }
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var agent = await agents.GetByIdAsync(id, cancellationToken);

        if (agent is null)
        {
            return NotFound();
        }

        return View(new AgentFormViewModel
        {
            Id = agent.Id,
            FullName = agent.FullName,
            Phone = agent.Phone,
            Email = agent.Email,
            ExperienceYears = agent.ExperienceYears
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(AgentFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var request = new UpdateAgentRequest(
                model.FullName,
                model.Phone,
                model.Email,
                model.ExperienceYears);

            var updated = await agents.UpdateAsync(model.Id, request, cancellationToken);

            if (updated is null)
            {
                return NotFound();
            }

            TempData["Success"] = "Агент обновлён.";
            return RedirectToAction(nameof(Index));
        }
        catch (DomainException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(model);
        }
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var agent = await agents.GetByIdAsync(id, cancellationToken);

        if (agent is null)
        {
            return NotFound();
        }

        return View(agent);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await agents.DeleteAsync(id, cancellationToken);

            if (!deleted)
            {
                return NotFound();
            }

            TempData["Success"] = "Агент удалён.";
        }
        catch (DomainException exception)
        {
            TempData["Error"] = exception.Message;
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "Нельзя удалить агента: за ним закреплены объекты.";
        }

        return RedirectToAction(nameof(Index));
    }
}
