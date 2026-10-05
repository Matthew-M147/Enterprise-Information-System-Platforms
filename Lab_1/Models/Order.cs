using System.ComponentModel.DataAnnotations.Schema;

[Table("orders")]
public class Order
{
    [Column("id")] public int Id { get; set; }
    [Column("client_id")] public int ClientId { get; set; }
    [Column("tour_id")] public int TourId { get; set; }
    [Column("order_date")] public DateOnly OrderDate { get; set; }
    [Column("trip_date")] public DateOnly TripDate { get; set; }
    [Column("quantity")] public int Quantity { get; set; }
    [Column("discount_percent", TypeName = "numeric(5,2)")] public decimal DiscountPercent { get; set; }

    public Client Client { get; set; } = null!;
    public Tour Tour { get; set; } = null!;

    public decimal Total => Math.Round(Tour.Price * Quantity * (1 - DiscountPercent / 100), 2);
}
