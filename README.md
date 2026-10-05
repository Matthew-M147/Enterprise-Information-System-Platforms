# Enterprise Information System Platforms

Лабораторні роботи з ПКІС. Кожна лаба — в окремій гілці.

- **lab1** — ADO.NET, варіант 14 «Туристична фірма»
- **lab2** — Entity Framework Core (на основі lab1)

**Стек:** C# (.NET 10), PostgreSQL 16, Npgsql, EF Core.

## Лабораторна 2

У цій лабі я переписав програму з першої лаби на Entity Framework Core.
База даних та завдання залишились ті самі — змінився лише спосіб роботи з БД.

Що я зробив:
- замінив пакет `Npgsql` на `Npgsql.EntityFrameworkCore.PostgreSQL`;
- описав таблиці класами `Client`, `Tour`, `Order` (папка `Models`);
- створив контекст `TravelAgencyContext` з підключенням до бази (папка `Data`);
- замість SQL-запитів і `DataReader` використав LINQ, а замість `JOIN` — `Include`.

**Запуск:** `dotnet run --project Lab_1`
(база `travel_agency` має бути створена — запустіть спочатку lab1 або виконайте `Lab_1/schema.sql`).
