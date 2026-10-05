public record OrderDto(
    int Id,
    int ClientId,
    string ClientName,
    int TourId,
    string TourName,
    DateOnly OrderDate,
    DateOnly TripDate,
    int DurationDays,
    int Quantity,
    decimal Price,
    decimal DiscountPercent,
    decimal Total);
