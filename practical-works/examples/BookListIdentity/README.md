# BookList + Identity — практическая работа № 7

Проект BookList из практических работ № 1–2, дополненный аутентификацией и авторизацией
на ASP.NET Core Identity. Всё делается по методичке, единственное отличие — вместо
PostgreSQL используется локальная SQLite, чтобы проект запускался без установленного сервера.

## Что сделано (по шагам методички)

| Шаг | Что реализовано | Файл |
|:--:|------|------|
| 1 | Пакеты `Microsoft.AspNetCore.Identity.EntityFrameworkCore`, EF Core, SQLite | `BookListIdentity.csproj` |
| 2 | Модель `ApplicationUser : IdentityUser` с `FullName` и `RegisteredAt` | `Models/ApplicationUser.cs` |
| 3 | В `Book` добавлены `OwnerId` и `Owner` | `Models/Book.cs` |
| 4 | `ApplicationDbContext : IdentityDbContext<ApplicationUser>`, индекс по `OwnerId`, `DeleteBehavior.SetNull` | `Data/ApplicationDbContext.cs` |
| 5 | Схема создаётся при старте (`EnsureCreated`), аналогично `dotnet ef database update` | `Data/DbInitializer.cs` |
| 6 | Проверка таблиц Identity — при запуске в `booklist.db` создаются `AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`, `AspNetUserClaims` и др. | — |
| 7 | Identity, роли, cookie, политика `OnlyAdults` по клейму `RegisteredAt` | `Program.cs` |
| 8 | Сидер: роли `Admin` и `User`, администратор и читатель | `Data/IdentitySeeder.cs` |
| 9–10 | Страницы входа, регистрации и профиля на русском языке | `Views/Account/*.cshtml` |
| 15 | Меню зависит от прав: приветствие, «Профиль», «Выйти», «Регистрация», «Войти» | `Views/Shared/_LoginPartial.cshtml` |
| 16 | Обычный пользователь видит только свои книги, администратор — все | `Controllers/BooksController.cs` |
| 17 | Создание, изменение и удаление — только для `Admin` | `Controllers/BooksController.cs` |
| 18 | Страница «Только для взрослых» и страница отказа | `Views/Home/Adults.cshtml`, `Views/Home/AccessDenied.cshtml` |

## Тестовые аккаунты

| Роль | Email | Пароль | Дата регистрации |
|------|-------|--------|------------------|
| Администратор | `admin@booklist.local` | `Admin123` | три года назад (проходит `OnlyAdults`) |
| Читатель | `user@booklist.local` | `User123` | сегодня (не проходит `OnlyAdults`) |

## Запуск

```bash
cd practical-works/examples/BookListIdentity
dotnet run
```

Приложение откроется на `https://localhost:7202`. База `booklist.db` создаётся автоматически.

## Как проверить

1. Войдите как администратор — видны все 8 книг и кнопка «Добавить книгу».
2. Войдите как читатель — список пуст, у него нет своих книг.
3. Откройте `/Home/Adults` под читателем — попадёте на «Доступ запрещён»; под администратором — откроется страница.
4. Под читателем откройте `/Books/Create` — доступ запрещён.
5. Создайте книгу под администратором — в карточке будет указан владелец.

## Примечание про Identity UI

В методичке страницы Identity предлагается scaffolding'ом (`dotnet aspnet-codegenerator identity`).
Здесь те же страницы (вход, регистрация, профиль, выход) написаны вручную на MVC —
функционально это то же самое, но не требует генератора и сразу на русском языке.
