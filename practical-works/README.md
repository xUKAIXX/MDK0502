# Практические работы — МДК 05.02

Учебный проект по МДК 05.02 «Разработка кода информационных систем».

Здесь два комплекта:

1. **`src/`, `tests/`, `docs/`** — последовательный проект по **варианту 3 «Агентство недвижимости»**:
   одно наращиваемое приложение, в котором реализованы все темы курса (ПР 1–7).
2. **[`examples/`](examples/)** — готовые примеры **по методичкам**, в том виде, в каком их
   диктует преподаватель: `BookList` (ПР 1–2), `BookListIdentity` (ПР 7), `MyApp` (ПР 12),
   `IntegrationLab` (ПР 13), `LibraryApp` (ПР 14). У каждого свой README с отчётом.

Если нужны именно работы «как в методичке» — открывайте `examples/`.
Если нужен проект по варианту — он ниже.

---

# Часть 1. Вариант 3 — «Агентство недвижимости»

## Что это

Одно последовательное ASP.NET Core-приложение, которое поэтапно наращивает функциональность:

1. простое MVC-приложение со списком объектов;
2. CRUD, поиск, сортировка, валидация, пагинация;
3. база данных через Entity Framework Core (локальная SQLite);
4. слои Domain / Application / Infrastructure, DTO и репозитории;
5. Web API, внедрение зависимостей, конфигурация, Swagger;
6. интеграция с внешним API через `HttpClient`;
7. аккаунты, роли, клеймы и авторизация через ASP.NET Core Identity.

## Структура

```text
practical-works/
├── README.md
├── RealEstateAgency.sln
├── src/
│   ├── RealEstateAgency.Domain/          # сущности и бизнес-правила
│   ├── RealEstateAgency.Application/     # DTO, сервисы, порты
│   ├── RealEstateAgency.Infrastructure/  # EF Core, SQLite, репозитории, Identity
│   ├── RealEstateAgency.Web/              # MVC + Web API + Swagger
│   ├── RealEstateAgency.PartnerApi/       # внешний «ипотечный» сервис
│   └── RealEstateAgency.IntegrationClient/# консольный клиент HttpClient
├── tests/
│   └── RealEstateAgency.Tests/            # xUnit-тесты бизнес-правил
└── docs/
    ├── pr01.md … pr07.md                  # отчёты по практикам
    └── README.md                          # соответствие практик и файлов
```

## Как запустить

Нужен установленный .NET 8 SDK. PostgreSQL не требуется: используется локальная SQLite, файл `realestate-agency.db` создаётся автоматически.

```bash
cd practical-works
dotnet restore
dotnet run --project src/RealEstateAgency.Web
```

Приложение запустится на `https://localhost:7200` (или `http://localhost:5200`, см. `Properties/launchSettings.json`). Swagger — по адресу `/swagger`.

### Тестовые аккаунты

| Роль | Email | Пароль |
|------|-------|--------|
| Администратор | `admin@agency.local` | `Admin123!` |
| Агент | `agent@agency.local` | `Agent123!` |
| Клиент | `client@agency.local` | `Client123!` |

### Внешний API и клиент интеграции

```bash
dotnet run --project src/RealEstateAgency.PartnerApi --urls http://localhost:5100
```

В отдельном терминале:

```bash
dotnet run --project src/RealEstateAgency.IntegrationClient
```

### Тесты

```bash
dotnet test
```

## Соответствие практическим работам

| Практика | Тема | Что реализовано | Отчёт |
|:--:|------|-----------------|-------|
| 1 | Простое веб-приложение ASP.NET Core MVC | Модель `Property`, список объектов, добавление через форму | [pr01.md](docs/pr01.md) |
| 2 | Расширение MVC: CRUD, поиск, валидация | Details / Edit / Delete, поиск, сортировка, пагинация, валидация | [pr02.md](docs/pr02.md) |
| 3 | Entity Framework Core и база данных | `ApplicationDbContext`, SQLite, конфигурации, сид данных, связи | [pr03.md](docs/pr03.md) |
| 4 | Модели, DTO и репозитории | Слои Domain / Application / Infrastructure, DTO, репозитории, Swagger | [pr04.md](docs/pr04.md) |
| 5 | Web API: слои, DI, Swagger, конфигурация | API-контроллеры, DI, `IOptions<T>`, Swagger, Polly | [pr05.md](docs/pr05.md) |
| 6 | Интеграция с внешним API через HttpClient | `PartnerApi`, типизированный клиент, retry, circuit breaker, идемпотентность | [pr06.md](docs/pr06.md) |
| 7 | Аутентификация и авторизация | Identity, роли, клеймы, политика `OnlyAdults`, владельцы объектов | [pr07.md](docs/pr07.md) |

## О нумерации файлов

В исходных методичках есть расхождение в названиях: файлы `pr12`, `pr13`, `pr14` по содержанию соответствуют более ранним темам (Web API, HttpClient, модели и репозитории), а `pr07` — теме аутентификации. В проекте темы разложены в логическом порядке: `pr01 → pr02 → pr03 → pr04 → pr05 → pr06 → pr07`. Номера практических работ в отчётах соответствуют этому порядку.

## Самопроверка проекта

Перед сдачей проект проверен статически (в окружении разработки отсутствовал .NET SDK, поэтому сборка и запуск выполняются на машине студента):

- все `*.cs` разобраны парсером C# — синтаксических ошибок нет;
- все `*.csproj` и `RealEstateAgency.sln` — корректный XML, пути `ProjectReference` разрешаются (на Linux используются `/`);
- в Razor-представлениях экранированы литеральные `@` (`@@`), каждый `asp-for` указывает на существующее свойство модели, каждый `asp-action` / `asp-controller` — на существующее действие;
- маршруты `api/...` не дублируются, контроллеры не конфликтуют;
- все интерфейсы реализованы своими сервисами, регистрация в DI-контейнере ссылается на существующие типы;
- в markdown-документах нет битых внутренних ссылок;
- `dotnet test` покрыты бизнес-правила `Property` и `Deal` (статусы, валидация, защита от редактирования проданных объектов).

---

# Часть 2. Примеры по методичкам

| Практика | Проект | Что внутри |
|:--:|--------|------------|
| 1–2 | [`examples/BookList`](examples/BookList/) | MVC-каталог книг: CRUD, поиск, сортировка, валидация, пагинация |
| 7 | [`examples/BookListIdentity`](examples/BookListIdentity/) | BookList + Identity: роли, клеймы, политика `OnlyAdults`, владельцы книг |
| 12 | [`examples/MyApp`](examples/MyApp/) | Web API: слои Domain/Application/Infrastructure/Api, DI, Swagger, `IOptions` |
| 13 | [`examples/IntegrationLab`](examples/IntegrationLab/) | `HttpClient`: типизированный клиент, Polly, идемпотентность, тесты |
| 14 | [`examples/LibraryApp`](examples/LibraryApp/) | EF Core, DTO, репозитории, «многие ко многим», тесты |

Каждый проект самодостаточен: своё решение, своя база SQLite, свой README с отчётом
и ответами на контрольные вопросы. Порты у всех разные, поэтому проекты можно
запускать одновременно.

```bash
cd practical-works/examples/BookList && dotnet run
```

## База данных и миграции

По умолчанию база создаётся автоматически через `EnsureCreatedAsync()`. Если нужно продемонстрировать миграции:

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --project src/RealEstateAgency.Infrastructure --startup-project src/RealEstateAgency.Web
dotnet ef database update --project src/RealEstateAgency.Infrastructure --startup-project src/RealEstateAgency.Web
```

Перед созданием миграций удалите файл `realestate-agency.db`, чтобы схема не конфликтовала с уже созданными таблицами.
