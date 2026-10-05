using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

[Table("tours")]
public class Tour
{
    [Column("id")]
    public int Id { get; set; }

    [Column("name"), Display(Name = "Назва")]
    [Required(ErrorMessage = "Вкажіть назву"), MaxLength(100)]
    public string Name { get; set; } = "";

    [Column("description"), Display(Name = "Опис")]
    [Required(ErrorMessage = "Вкажіть опис")]
    public string Description { get; set; } = "";

    [Column("price", TypeName = "numeric(10,2)"), Display(Name = "Ціна, грн")]
    [Range(0.01, 10_000_000, ErrorMessage = "Ціна має бути більшою за 0")]
    public decimal Price { get; set; }

    [Column("duration_days"), Display(Name = "Тривалість, днів")]
    [Range(1, 365, ErrorMessage = "Від 1 до 365 днів")]
    public int DurationDays { get; set; }

    [ValidateNever]
    public List<Order> Orders { get; set; } = new();
}
