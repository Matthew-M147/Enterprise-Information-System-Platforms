using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

[Table("orders")]
public class Order
{
    [Column("id")]
    public int Id { get; set; }

    [Column("client_id"), Display(Name = "Клієнт")]
    [Range(1, int.MaxValue, ErrorMessage = "Оберіть клієнта")]
    public int ClientId { get; set; }

    [Column("tour_id"), Display(Name = "Тур")]
    [Range(1, int.MaxValue, ErrorMessage = "Оберіть тур")]
    public int TourId { get; set; }

    [Column("order_date"), Display(Name = "Дата замовлення")]
    public DateOnly OrderDate { get; set; }

    [Column("trip_date"), Display(Name = "Дата поїздки")]
    public DateOnly TripDate { get; set; }

    [Column("quantity"), Display(Name = "Кількість")]
    [Range(1, 100, ErrorMessage = "Від 1 до 100")]
    public int Quantity { get; set; }

    [Column("discount_percent", TypeName = "numeric(5,2)"), Display(Name = "Знижка, %")]
    [Range(0, 100, ErrorMessage = "Від 0 до 100")]
    public decimal DiscountPercent { get; set; }

    [ValidateNever]
    public Client Client { get; set; } = null!;

    [ValidateNever]
    public Tour Tour { get; set; } = null!;

    [ValidateNever]
    public decimal Total => Math.Round(Tour.Price * Quantity * (1 - DiscountPercent / 100), 2);
}
