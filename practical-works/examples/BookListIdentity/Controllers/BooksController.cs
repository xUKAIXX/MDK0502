using BookList.Data;
using BookList.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookList.Controllers;

[Authorize]
public class BooksController : Controller
{
    private const int PageSize = 5;

    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public BooksController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // GET: /Books/Index?search=...&minYear=...&maxYear=...&page=...
    [AllowAnonymous]
    public async Task<IActionResult> Index(
        string? search,
        int? minYear,
        int? maxYear,
        int page = 1)
    {
        // Асинхронный запрос без отслеживания изменений: достаточно только чтения.
        var query = _db.Books.AsNoTracking();

        // Разграничение доступа: администратор видит все книги,
        // обычный пользователь — только те, где он владелец.
        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && User.Identity?.IsAuthenticated == true)
        {
            var userId = _userManager.GetUserId(User);
            query = query.Where(book => book.OwnerId == userId);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(book => book.Title.Contains(search));
        }

        if (minYear.HasValue)
        {
            query = query.Where(book => book.Year >= minYear.Value);
        }

        if (maxYear.HasValue)
        {
            query = query.Where(book => book.Year <= maxYear.Value);
        }

        var total = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(total / (double)PageSize);
        page = Math.Clamp(page, 1, Math.Max(1, totalPages));

        var books = await query
            .OrderBy(book => book.Year)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Include(book => book.Owner)
            .ToListAsync();

        ViewBag.Search = search;
        ViewBag.MinYear = minYear;
        ViewBag.MaxYear = maxYear;
        ViewBag.Page = page;
        ViewBag.Total = total;
        ViewBag.TotalPages = Math.Max(1, totalPages);
        ViewBag.IsAdmin = isAdmin;

        return View(books);
    }

    // GET: /Books/Details/5
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var book = await _db.Books
            .AsNoTracking()
            .Include(b => b.Owner)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // GET: /Books/Create
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Books/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Book book)
    {
        if (ModelState.IsValid)
        {
            // Идентификатор владельца проставляется автоматически из текущего пользователя.
            book.OwnerId = _userManager.GetUserId(User);

            _db.Books.Add(book);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(book);
    }

    // GET: /Books/Edit/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // POST: /Books/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(Book book)
    {
        if (ModelState.IsValid)
        {
            var existing = await _db.Books.FirstOrDefaultAsync(b => b.Id == book.Id);

            if (existing == null)
            {
                return NotFound();
            }

            existing.Title = book.Title;
            existing.Author = book.Author;
            existing.Genre = book.Genre;
            existing.Year = book.Year;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(book);
    }

    // GET: /Books/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // POST: /Books/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == id);

        if (book != null)
        {
            _db.Books.Remove(book);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
