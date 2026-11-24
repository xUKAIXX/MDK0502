using BookList.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookList.Controllers;

public class BooksController : Controller
{
    // Хранение данных — статический список в памяти (по методичке).
    private static readonly List<Book> _books = new()
    {
        new Book { Id = 1, Title = "Война и мир", Author = "Толстой", Year = 1869, Genre = "Роман" },
        new Book { Id = 2, Title = "Преступление и наказание", Author = "Достоевский", Year = 1866, Genre = "Роман" },
        new Book { Id = 3, Title = "Мастер и Маргарита", Author = "Булгаков", Year = 1967, Genre = "Фантастика" },
        new Book { Id = 4, Title = "Отцы и дети", Author = "Тургенев", Year = 1862, Genre = "Роман" },
        new Book { Id = 5, Title = "Собачье сердце", Author = "Булгаков", Year = 1925, Genre = "Повесть" },
        new Book { Id = 6, Title = "Евгений Онегин", Author = "Пушкин", Year = 1833, Genre = "Поэзия" },
        new Book { Id = 7, Title = "Тихий Дон", Author = "Шолохов", Year = 1940, Genre = "Роман" },
        new Book { Id = 8, Title = "Два капитана", Author = "Каверин", Year = 1944, Genre = "Приключения" }
    };

    private static int _nextId = 9;

    private const int PageSize = 3;

    // GET: /Books/Index?search=...&genre=...&sortOrder=...&page=...
    public IActionResult Index(string? search, string? genre, string? sortOrder, int page = 1)
    {
        var books = _books.AsQueryable();

        // Поиск по названию или автору.
        if (!string.IsNullOrWhiteSpace(search))
        {
            books = books.Where(b =>
                b.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                b.Author.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        // Индивидуальное задание 1: фильтрация по жанру.
        if (!string.IsNullOrWhiteSpace(genre))
        {
            books = books.Where(b => b.Genre == genre);
        }

        // Сортировка. Направление переключается повторным нажатием на заголовок.
        ViewData["YearSort"] = string.IsNullOrEmpty(sortOrder) ? "year_desc" : "";
        ViewData["TitleSort"] = sortOrder == "title" ? "title_desc" : "title";
        ViewData["AuthorSort"] = sortOrder == "author" ? "author_desc" : "author";

        books = sortOrder switch
        {
            "year_desc" => books.OrderByDescending(b => b.Year),
            "title" => books.OrderBy(b => b.Title),
            "title_desc" => books.OrderByDescending(b => b.Title),
            "author" => books.OrderBy(b => b.Author),
            "author_desc" => books.OrderByDescending(b => b.Author),
            _ => books.OrderBy(b => b.Year)
        };

        // Индивидуальное задание 3: пагинация по 3 книги на страницу.
        var total = books.Count();
        var totalPages = (int)Math.Ceiling(total / (double)PageSize);
        page = Math.Clamp(page, 1, Math.Max(1, totalPages));

        var pageItems = books
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToList();

        // Текущее состояние фильтров — для формы и ссылок сортировки.
        ViewData["CurrentSort"] = sortOrder;
        ViewData["CurrentFilter"] = search;
        ViewData["CurrentGenre"] = genre;
        ViewData["Genres"] = _books.Select(b => b.Genre).Distinct().OrderBy(g => g).ToList();
        ViewData["TotalCount"] = total;
        ViewData["Page"] = page;
        ViewData["TotalPages"] = Math.Max(1, totalPages);

        return View(pageItems);
    }

    // GET: /Books/Details/5
    public IActionResult Details(int id)
    {
        var book = _books.FirstOrDefault(b => b.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // GET: /Books/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Books/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Book book)
    {
        if (ModelState.IsValid)
        {
            book.Id = _nextId++;
            _books.Add(book);
            return RedirectToAction(nameof(Index));
        }

        return View(book);
    }

    // GET: /Books/Edit/5
    public IActionResult Edit(int id)
    {
        var book = _books.FirstOrDefault(b => b.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // POST: /Books/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Book book)
    {
        if (ModelState.IsValid)
        {
            var existing = _books.FirstOrDefault(b => b.Id == book.Id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.Title = book.Title;
            existing.Author = book.Author;
            existing.Year = book.Year;
            existing.Genre = book.Genre;
            return RedirectToAction(nameof(Index));
        }

        return View(book);
    }

    // GET: /Books/Delete/5
    public IActionResult Delete(int id)
    {
        var book = _books.FirstOrDefault(b => b.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // POST: /Books/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var book = _books.FirstOrDefault(b => b.Id == id);
        if (book != null)
        {
            _books.Remove(book);
        }

        return RedirectToAction(nameof(Index));
    }
}
