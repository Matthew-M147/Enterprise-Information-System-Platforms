# Enterprise Information System Platforms

Лабораторні роботи з ПКІС. Кожна лаба — в окремій гілці.

- **lab1** — ADO.NET, варіант 14 «Туристична фірма»
- **lab2** — Entity Framework Core (на основі lab1)
- **lab3** — ASP.NET Core MVC (на основі lab2)
- **lab4** — ASP.NET Core Razor Pages + Web API (на основі lab3)
- **lab5** — Blazor (на основі lab4)
- **lab6** — gRPC-сервіс + консольний клієнт (на основі lab5)
- **lab7** — .NET MAUI, застосунок для Windows (на основі lab6)
- **lab8** — ASP.NET Core Web API + інтерфейс на Angular (TypeScript) (на основі lab7)

**Стек:** C# (.NET 10), PostgreSQL 16, Npgsql, EF Core, ASP.NET Core (MVC, Razor Pages, Web API, Blazor, gRPC), .NET MAUI, Swagger, TypeScript, Angular 20.

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

## Лабораторна 5

У цій лабі я переписав інтерфейс на Blazor — це альтернативний спосіб зробити той самий CRUD.
Razor Pages і Web API з четвертої лаби тут не використовуються (вони залишились у гілці `lab4`).

Що я зробив:
- створив Blazor-компоненти (папка `Components`): кожен `.razor` файл містить і розмітку, і C#-код у блоці `@code`;
- для кожної сутності зробив список (`ClientList`, `TourList`, `OrderList`) і одну форму для створення та редагування (`ClientForm`, `TourForm`, `OrderForm`);
- використав режим Interactive Server: кнопки й форми обробляються на C# без перезавантаження сторінки;
- зробив видалення з підтвердженням прямо в рядку таблиці;
- для роботи з базою використав `IDbContextFactory` — окремий контекст на кожну операцію, як рекомендує Microsoft для Blazor.

## Лабораторна 6

У цій лабі я зробив той самий CRUD через спеціалізований веб-сервіс — gRPC.
Сервер не має графічного інтерфейсу: з ним працює окрема консольна програма-клієнт.
Blazor з п'ятої лаби тут не використовується (він залишився в гілці `lab5`).

Що я зробив:
- описав контракт сервісу у файлі `Lab_1/Protos/travel_agency.proto`: повідомлення (`ClientMessage`, `TourMessage`, `OrderMessage`) і сервіси `ClientService`, `TourService`, `OrderService` з методами `GetAll`, `Get`, `Create`, `Update`, `Delete`;
- реалізував сервіси на сервері (папка `Lab_1/Services`): вони працюють з базою через EF Core, а помилки повертають статусами gRPC (`NotFound`, `InvalidArgument`);
- увімкнув HTTP/2, бо gRPC працює тільки через нього;
- створив консольний клієнт `GrpcClient` з меню, який через gRPC показує, додає, редагує та видаляє клієнтів, тури й замовлення; код для викликів генерується з того самого `.proto`.

## Лабораторна 7

У цій лабі я зробив той самий CRUD як настільний застосунок на .NET MAUI (для Windows).
Це вже не сайт і не сервіс, а програма з власним вікном, яка працює з базою напряму через EF Core.
gRPC-сервер і консольний клієнт з шостої лаби тут не використовуються (вони залишились у гілці `lab6`).

Що я зробив:
- перетворив проєкт на MAUI-застосунок (`UseMaui`, ціль `net10.0-windows`); налаштування — у `MauiProgram.cs`;
- зробив навігацію вкладками «Клієнти», «Тури», «Замовлення» (`AppShell.xaml`, `Shell` + `TabBar`);
- створив сторінки (папка `Pages`): кожна складається з розмітки `.xaml` і коду `.xaml.cs`;
- для кожної сутності зробив список (`ClientsPage`, `ToursPage`, `OrdersPage`) на `CollectionView` з кнопками «Редагувати» / «Видалити» і форму для створення та редагування (`ClientEditPage`, `TourEditPage`, `OrderEditPage`);
- у формі замовлення клієнт і тур вибираються зі списків (`Picker`), а дати — у календарі (`DatePicker`);
- перевіряю введені дані за атрибутами моделей і показую помилки під формою; видалення підтверджується діалогом;
- для роботи з базою використав `IDbContextFactory`, як і в п'ятій лабі.

## Лабораторна 8

У цій лабі я розділив застосунок на дві частини: бекенд на ASP.NET Core Web API і фронтенд на Angular (TypeScript).
Фронтенд працює в браузері й отримує та змінює дані тільки через HTTP-запити до API.
MAUI-застосунок із сьомої лаби тут не використовується (він залишився в гілці `lab7`).

Що я зробив:
- повернув у `Lab_1` Web API: контролери `api/clients`, `api/tours`, `api/orders` з запитами `GET`, `POST`, `PUT`, `DELETE` і DTO у відповідях (папки `Controllers`, `Dtos`); база — через EF Core;
- створив Angular-застосунок у папці `client`:
  - `models` — TypeScript-інтерфейси `Client`, `Tour`, `Order`, що відповідають DTO з API;
  - `services` — базовий клас `CrudService<T>` на `HttpClient` і сервіси `ClientsService`, `ToursService`, `OrdersService`;
  - компоненти списку й форми для кожної сутності (`client-list`, `client-form` тощо) з маршрутами `/clients`, `/clients/new`, `/clients/:id/edit`;
- помилки валідації з API показуються під відповідними полями форми, а видалення підтверджується прямо в рядку таблиці;
- під час розробки `ng serve` перенаправляє запити `/api` на Web API (`proxy.conf.json`), а `ng build` збирає фронтенд у `Lab_1/wwwroot`, щоб його міг роздавати сам API.

## Запуск

Потрібен PostgreSQL (користувач `postgres` / пароль `admin`) і база `travel_agency`,
яку створює lab1 або скрипт `Lab_1/schema.sql`.

```bash
dotnet run --project Lab_1
```

- lab1, lab2 — результат виводиться в консоль;
- lab3, lab5 — відкривається сайт http://localhost:5080
- lab4 — сайт http://localhost:5080 та API http://localhost:5080/swagger
- lab6 — запускається gRPC-сервер на http://localhost:5080; після цього в іншому терміналі запустіть клієнт:

```bash
dotnet run --project GrpcClient
```

- lab7 — відкривається вікно застосунку «Туристична фірма». Для збірки потрібен workload MAUI для Windows:

```bash
dotnet workload install maui-windows
```

- lab8 — запускається Web API на http://localhost:5080; потрібен Node.js. В іншому терміналі запустіть Angular і відкрийте http://localhost:4200:

```bash
cd client
npm install
npm start
```

Або зберіть фронтенд один раз (`npm run build` у папці `client`) — тоді після `dotnet run --project Lab_1` сайт буде доступний на http://localhost:5080.
