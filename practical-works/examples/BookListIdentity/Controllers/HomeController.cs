using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookList.Controllers;

public class HomeController : Controller
{
    [AllowAnonymous]
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Books");
    }

    // Страница под собственной политикой «Только для взрослых».
    [Authorize(Policy = "OnlyAdults")]
    public IActionResult Adults()
    {
        return View();
    }

    // Сюда приложение перенаправляет, когда проверка авторизации не пройдена.
    [AllowAnonymous]
    public IActionResult AccessDenied()
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
