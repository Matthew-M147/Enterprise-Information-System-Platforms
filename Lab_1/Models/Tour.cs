using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("tours")]
public class Tour
{
    [Column("id")] public int Id { get; set; }
    [Column("name"), MaxLength(100)] public string Name { get; set; } = "";
    [Column("description")] public string Description { get; set; } = "";
    [Column("price", TypeName = "numeric(10,2)")] public decimal Price { get; set; }
    [Column("duration_days")] public int DurationDays { get; set; }

    public List<Order> Orders { get; set; } = new();
}
