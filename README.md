# Enterprise Information System Platforms

Лабораторні роботи з ПКІС. Кожна лаба — в окремій гілці.

- **lab1** — ADO.NET, варіант 14 «Туристична фірма»
- **lab2** — Entity Framework Core (на основі lab1)
- **lab3** — ASP.NET Core MVC (на основі lab2)
- **lab4** — ASP.NET Core Razor Pages + Web API (на основі lab3)

**Стек:** C# (.NET 10), PostgreSQL 16, Npgsql, EF Core, ASP.NET Core (MVC, Razor Pages, Web API), Swagger.

## Лабораторна 2

У цій лабі я переписав програму з першої лаби на Entity Framework Core.
База даних та завдання залишились ті самі — змінився лише спосіб роботи з БД.

Що я зробив:
- замінив пакет `Npgsql` на `Npgsql.EntityFrameworkCore.PostgreSQL`;
- описав таблиці класами `Client`, `Tour`, `Order` (папка `Models`);
- створив контекст `TravelAgencyContext` з підключенням до бази (папка `Data`);
- замість SQL-запитів і `DataReader` використав LINQ, а замість `JOIN` — `Include`.

## Лабораторна 3

У цій лабі я перетворив консольну програму з другої лаби на веб-застосунок ASP.NET Core MVC.
Тепер клієнтів, тури й замовлення можна не лише переглядати, а й додавати, редагувати та видаляти.

Що я зробив:
- змінив тип проєкту на веб (`Microsoft.NET.Sdk.Web`);
- додав контролери `Clients`, `Tours`, `Orders` з діями `Index` (перегляд), `Create`, `Edit`, `Delete`;
- створив сторінки (Views) зі списками, формами та підтвердженням видалення;
- додав у моделі підписи полів і перевірку введених даних (`Display`, `Required`, `Range`);
- переніс рядок підключення в `appsettings.json`, а контекст підключив через DI (`AddDbContext`).

## Лабораторна 4

У цій лабі я замінив MVC на Razor Pages і додав Web API.
Сайт працює так само, а дані тепер доступні ще й у форматі JSON для інших програм.

Що я зробив:
- замінив контролери та Views на Razor Pages (папка `Pages`): кожна сторінка — це `.cshtml` і код `.cshtml.cs` з методами `OnGet` / `OnPost`;
- додав API-контролери з `[ApiController]`: `api/clients`, `api/tours`, `api/orders` з запитами `GET`, `POST`, `PUT`, `DELETE`;
- зробив DTO (папка `Dtos`), щоб API повертав зручні дані — наприклад, замовлення одразу з ім'ям клієнта, назвою туру та сумою;
- підключив Swagger для перевірки API в браузері.

## Запуск

Потрібен PostgreSQL (користувач `postgres` / пароль `admin`) і база `travel_agency`,
яку створює lab1 або скрипт `Lab_1/schema.sql`.

```bash
dotnet run --project Lab_1
```

- lab1, lab2 — результат виводиться в консоль;
- lab3 — відкривається сайт http://localhost:5080
- lab4 — сайт http://localhost:5080 та API http://localhost:5080/swagger
