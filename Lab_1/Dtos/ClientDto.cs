public record ClientDto(
    int Id,
    string LastName,
    string FirstName,
    string? MiddleName,
    string Phone,
    string City,
    string Street,
    string Building,
    string? Apartment,
    int OrdersCount);
