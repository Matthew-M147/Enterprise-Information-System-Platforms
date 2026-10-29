namespace Lab_1.Dtos;

public record TourDto(
    int Id,
    string Name,
    string Description,
    decimal Price,
    int DurationDays,
    int OrdersCount);
