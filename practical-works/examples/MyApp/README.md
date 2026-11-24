# MyApp — практическая работа № 12

Серверная часть информационной системы на ASP.NET Core Web API: слои, внедрение
зависимостей, Swagger и конфигурация. Решение сделано по методичке.

## Структура решения

| Проект | Ссылается на | Содержит |
|--------|--------------|----------|
| `MyApp.Domain` | — | `Order`, `OrderItem`, `OrderStatus`, `DomainException` |
| `MyApp.Application` | Domain | `IOrderService`, `OrderService`, `IOrderRepository`, DTO, `OrderOptions`, `IClock` |
| `MyApp.Infrastructure` | Application | `InMemoryOrderRepository`, `SystemClock`, `DependencyInjection` |
| `MyApp.Api` | Application, Infrastructure | `OrdersController`, `Program.cs`, `appsettings.json` |

Правило зависимостей: стрелки направлены только внутрь, к доменной модели.
В проекте `MyApp.Domain` нет ни одной ссылки на EF Core или ASP.NET Core —
проверяется компилятором, потому что слои разделены проектами, а не папками.

## Эндпоинты

| Метод | Адрес | Что делает |
|-------|-------|------------|
| `POST` | `/api/orders` | создаёт заказ, возвращает 201 и `Guid` |
| `GET` | `/api/orders/{id}` | заказ по идентификатору, 404 если не найден |
| `GET` | `/api/orders` | список всех заказов |
| `POST` | `/api/orders/{id}/submit` | отправляет черновик, 400 при нарушении бизнес-правил |
| `GET` | `/api/diagnostics/options` | показывает применённые настройки `OrderOptions` |

## Конфигурация

`appsettings.json`:

```json
{
  "Orders": {
    "MaxItemsPerOrder": 5,
    "DefaultCurrency": "RUB"
  }
}
```

Настройки читаются через `IOptions<OrderOptions>`. Перекрыть параметр без правки файла
можно переменной среды:

```powershell
$env:Orders__MaxItemsPerOrder = "1"
dotnet run --project src/MyApp.Api
```

После перезапуска `GET /api/diagnostics/options` вернёт `MaxItemsPerOrder = 1`,
а вторая позиция в заказе даст 400 «В заказе не может быть больше 1 позиций».

## Запуск

```bash
cd practical-works/examples/MyApp
dotnet run --project src/MyApp.Api
```

- Swagger UI — `http://localhost:5080/swagger`
- Диагностика настроек — `http://localhost:5080/api/diagnostics/options`

## Эксперименты по проверке DI (из методички)

| Эксперимент | Что изменить | Ожидаемый результат |
|-------------|--------------|---------------------|
| Захват зависимости | `AddSingleton<IOrderService, OrderService>()` | ошибка проверки областей в Development |
| Подмена реализации | другая реализация `IOrderRepository` | контроллер и сервис не меняются |
| Приоритет настроек | `Orders__MaxItemsPerOrder=1` | вторая позиция даёт 400 |
| Отсутствие настройки | удалить раздел `Orders` | значения по умолчанию (10, RUB) |

## Ответы на контрольные вопросы

1. **Почему Domain не должен ссылаться на EF Core?** Домен описывает предметную область, а способ хранения — деталь инфраструктуры. Иначе бизнес-правила окажутся привязаны к провайдеру БД и их нельзя будет проверить без базы.
2. **Чем слои проектами лучше папок?** Компилятор проверяет направление зависимостей: ссылка из Domain на Infrastructure просто не соберётся. В папках можно случайно импортировать что угодно.
3. **Порт и адаптер.** Порт — интерфейс в Application (`IOrderRepository`, `IClock`). Адаптер — реализация в Infrastructure (`InMemoryOrderRepository`, `SystemClock`).
4. **Зачем `IClock`?** Абстрагирует время: в тестах подставляется фиксированное время, в работе — `DateTime.UtcNow`.
5. **Времена жизни служб.** Singleton — один объект на приложение; Scoped — один на запрос; Transient — новый на каждое разрешение. `DbContext` обычно Scoped.
6. **DbContext как Singleton.** Он не потокобезопасен: параллельные запросы приведут к ошибкам и утечке состояния.
7. **Выбор конструктора.** Контейнер выбирает конструктор с наибольшим числом параметров, которые умеет разрешить; можно указать `[ActivatorUtilitiesConstructor]`.
8. **Источники конфигурации.** `appsettings.json` → `appsettings.{Environment}.json` → user secrets → переменные среды → аргументы командной строки. Последний перекрывает предыдущие.
9. **`IOptions<T>` вместо `IConfiguration`.** Даёт типизированный доступ, валидацию и пересчёт значений без перезапуска; `IConfiguration` возвращает строки.
10. **`ProducesResponseType`.** Описывает коды и типы ответов, Swagger показывает их в документации.
11. **Почему доменное исключение → 400.** Это ошибка входных данных или бизнес-правил, клиент может исправить запрос. 500 означал бы сбой сервера.
12. **Переход на EF Core и PostgreSQL.** Меняется только `DependencyInjection` в Infrastructure: вместо `InMemoryOrderRepository` регистрируется EF-репозиторий и `DbContext`; контроллеры и сервисы не меняются.
