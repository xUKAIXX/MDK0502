# Готовые примеры по методичкам

Здесь лежат отдельные проекты, сделанные **по примерам из методичек** — в том виде,
в каком их диктует преподаватель. Каждый проект самодостаточен: у него своё решение,
своя база и свой отчёт в README.

| Практика | Проект | Что внутри | Отчёт |
|:--:|--------|------------|-------|
| 1–2 | [`BookList/`](BookList/) | MVC-каталог книг: CRUD, поиск, сортировка, валидация, пагинация | [README](BookList/README.md) |
| 7 | [`BookListIdentity/`](BookListIdentity/) | BookList + Identity: роли, клеймы, политика `OnlyAdults`, владельцы книг | [README](BookListIdentity/README.md) |
| 12 | [`MyApp/`](MyApp/) | Web API: слои Domain/Application/Infrastructure/Api, DI, Swagger, `IOptions` | [README](MyApp/README.md) |
| 13 | [`IntegrationLab/`](IntegrationLab/) | `HttpClient`: типизированный клиент, Polly, идемпотентность, тесты | [README](IntegrationLab/README.md) |
| 14 | [`LibraryApp/`](LibraryApp/) | EF Core, DTO, репозитории, индивидуальное задание с «многими ко многим» | [README](LibraryApp/README.md) |

## Порты

| Проект | Адрес |
|--------|-------|
| BookList | `https://localhost:7201` |
| BookListIdentity | `https://localhost:7202` |
| MyApp.Api | `http://localhost:5080` |
| PartnerApi (IntegrationLab) | `http://localhost:5100` |
| LibraryApp.Api | `http://localhost:5090` |

Порты выбраны разными, чтобы проекты можно было запускать одновременно.

## Базы данных

В методичках используется PostgreSQL. Здесь во всех проектах стоит **локальная SQLite** —
файл базы создаётся автоматически, ничего устанавливать не нужно. Если преподаватель
требует именно PostgreSQL, достаточно заменить провайдер в `Program.cs` / `DependencyInjection.cs`
на `UseNpgsql` и указать строку подключения.

## Как запускать

Каждый проект запускается из своей папки:

```bash
cd practical-works/examples/BookList
dotnet run
```

Тесты там, где они есть:

```bash
dotnet test
```
