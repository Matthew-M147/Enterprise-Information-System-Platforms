using Grpc.Core;

public abstract class CrudMenu
{
    protected abstract string Title { get; }

    protected abstract Task ShowAllAsync();
    protected abstract Task CreateAsync();
    protected abstract Task EditAsync();
    protected abstract Task DeleteAsync();

    public async Task RunAsync()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine($"===== {Title} =====");
            Console.WriteLine("1. Показати всі");
            Console.WriteLine("2. Додати");
            Console.WriteLine("3. Редагувати");
            Console.WriteLine("4. Видалити");
            Console.WriteLine("0. Назад");

            var choice = ConsoleInput.ReadChoice();
            Console.WriteLine();
            try
            {
                switch (choice)
                {
                    case "1": await ShowAllAsync(); break;
                    case "2": await CreateAsync(); break;
                    case "3": await EditAsync(); break;
                    case "4": await DeleteAsync(); break;
                    case "0": return;
                    default: Console.WriteLine("Невідомий пункт меню."); break;
                }
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Помилка ({ex.StatusCode}): {ex.Status.Detail}");
            }
        }
    }
}
