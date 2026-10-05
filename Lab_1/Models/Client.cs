using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("clients")]
public class Client
{
    [Column("id")] public int Id { get; set; }
    [Column("last_name"), MaxLength(50)] public string LastName { get; set; } = "";
    [Column("first_name"), MaxLength(50)] public string FirstName { get; set; } = "";
    [Column("middle_name"), MaxLength(50)] public string? MiddleName { get; set; }
    [Column("phone"), MaxLength(20)] public string Phone { get; set; } = "";
    [Column("city"), MaxLength(50)] public string City { get; set; } = "";
    [Column("street"), MaxLength(100)] public string Street { get; set; } = "";
    [Column("building"), MaxLength(10)] public string Building { get; set; } = "";
    [Column("apartment"), MaxLength(10)] public string? Apartment { get; set; }

    public List<Order> Orders { get; set; } = new();

    public string FullName => $"{LastName} {FirstName} {MiddleName}";
    public string Address => $"{City}, {Street}, {Building}" + (Apartment != null ? $", кв. {Apartment}" : "");
}
