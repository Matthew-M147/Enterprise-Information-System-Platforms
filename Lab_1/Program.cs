using System.Text;
using Npgsql;

Console.OutputEncoding = Encoding.UTF8;

const string DatabaseName = "travel_agency";
const string ServerConnectionString = "Host=localhost;Port=5432;Username=postgres;Password=admin;Database=postgres";
const string ConnectionString = $"Host=localhost;Port=5432;Username=postgres;Password=admin;Database={DatabaseName}";

EnsureDatabase();

using (var connection = new NpgsqlConnection(ConnectionString))
{
    connection.Open();
    Console.WriteLine("Підключення до бази даних успішне!\n");

    ShowClients(connection);
    ShowTours(connection);
    ShowOrders(connection);
}

void ShowClients(NpgsqlConnection connection)
{
    string query = @"
        SELECT id, last_name, first_name, middle_name, phone,
               city || ', ' || street || ', ' || building
                   || COALESCE(', кв. ' || apartment, '') AS address
        FROM clients
        ORDER BY id;";

    using (var command = new NpgsqlCommand(query, connection))
    using (var reader = command.ExecuteReader())
    {
        Console.WriteLine("===== Клієнти =====");
        Console.WriteLine($"{"ID",-4}{"ПІБ",-38}{"Телефон",-16}{"Адреса"}");
        Console.WriteLine(new string('-', 110));

        while (reader.Read())
        {
            string fullName = $"{reader["last_name"]} {reader["first_name"]} {reader["middle_name"]}";
            Console.WriteLine(
                $"{reader["id"],-4}" +
                $"{fullName,-38}" +
                $"{reader["phone"],-16}" +
                $"{reader["address"]}");
        }
        Console.WriteLine();
    }
}

void ShowTours(NpgsqlConnection connection)
{
    string query = "SELECT id, name, price, duration_days, description FROM tours ORDER BY id;";

    using (var command = new NpgsqlCommand(query, connection))
    using (var reader = command.ExecuteReader())
    {
        Console.WriteLine("===== Тури =====");

        while (reader.Read())
        {
            Console.WriteLine($"[{reader["id"]}] {reader["name"]}");
            Console.WriteLine($"    Ціна: {(decimal)reader["price"]:N2} грн, тривалість: {reader["duration_days"]} дн.");
            Console.WriteLine($"    Опис: {reader["description"]}");
        }
        Console.WriteLine();
    }
}

void ShowOrders(NpgsqlConnection connection)
{
    string query = @"
        SELECT o.id, c.last_name, c.first_name, t.name AS tour,
               o.trip_date, t.duration_days, o.quantity, t.price, o.discount_percent,
               ROUND(t.price * o.quantity * (1 - o.discount_percent / 100), 2) AS total
        FROM orders o
        JOIN clients c ON o.client_id = c.id
        JOIN tours   t ON o.tour_id   = t.id
        ORDER BY c.id, o.trip_date;";

    using (var command = new NpgsqlCommand(query, connection))
    using (var reader = command.ExecuteReader())
    {
        Console.WriteLine("===== Замовлення =====");
        Console.WriteLine($"{"№",-4}{"Клієнт",-22}{"Тур",-22}{"Дата поїздки",-14}{"Днів",-6}{"К-сть",-7}{"Ціна",-12}{"Знижка",-8}{"Сума"}");
        Console.WriteLine(new string('-', 110));

        while (reader.Read())
        {
            string client = $"{reader["last_name"]} {reader["first_name"]}";
            Console.WriteLine(
                $"{reader["id"],-4}" +
                $"{client,-22}" +
                $"{reader["tour"],-22}" +
                $"{(DateOnly)reader["trip_date"],-14:dd.MM.yyyy}" +
                $"{reader["duration_days"],-6}" +
                $"{reader["quantity"],-7}" +
                $"{(decimal)reader["price"],-12:N2}" +
                $"{((decimal)reader["discount_percent"]).ToString("0.##") + "%",-8}" +
                $"{(decimal)reader["total"]:N2}");
        }
    }
}

void EnsureDatabase()
{
    using (var connection = new NpgsqlConnection(ServerConnectionString))
    {
        connection.Open();
        using var check = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname = '{DatabaseName}'", connection);
        if (check.ExecuteScalar() == null)
        {
            using var create = new NpgsqlCommand($"CREATE DATABASE {DatabaseName} ENCODING 'UTF8'", connection);
            create.ExecuteNonQuery();
        }
    }

    using (var connection = new NpgsqlConnection(ConnectionString))
    {
        connection.Open();
        using var check = new NpgsqlCommand("SELECT to_regclass('public.orders') IS NOT NULL", connection);
        if (!(bool)check.ExecuteScalar()!)
        {
            string script = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "schema.sql"));
            using var command = new NpgsqlCommand(script, connection);
            command.ExecuteNonQuery();
        }
    }
}
