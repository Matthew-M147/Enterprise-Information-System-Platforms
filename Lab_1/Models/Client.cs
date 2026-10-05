using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

[Table("clients")]
public class Client
{
    [Column("id")]
    public int Id { get; set; }

    [Column("last_name"), Display(Name = "Прізвище")]
    [Required(ErrorMessage = "Вкажіть прізвище"), MaxLength(50)]
    public string LastName { get; set; } = "";

    [Column("first_name"), Display(Name = "Ім'я")]
    [Required(ErrorMessage = "Вкажіть ім'я"), MaxLength(50)]
    public string FirstName { get; set; } = "";

    [Column("middle_name"), Display(Name = "По батькові")]
    [MaxLength(50)]
    public string? MiddleName { get; set; }

    [Column("phone"), Display(Name = "Телефон")]
    [Required(ErrorMessage = "Вкажіть телефон"), MaxLength(20)]
    public string Phone { get; set; } = "";

    [Column("city"), Display(Name = "Місто")]
    [Required(ErrorMessage = "Вкажіть місто"), MaxLength(50)]
    public string City { get; set; } = "";

    [Column("street"), Display(Name = "Вулиця")]
    [Required(ErrorMessage = "Вкажіть вулицю"), MaxLength(100)]
    public string Street { get; set; } = "";

    [Column("building"), Display(Name = "Будинок")]
    [Required(ErrorMessage = "Вкажіть будинок"), MaxLength(10)]
    public string Building { get; set; } = "";

    [Column("apartment"), Display(Name = "Квартира")]
    [MaxLength(10)]
    public string? Apartment { get; set; }

    [ValidateNever, JsonIgnore]
    public List<Order> Orders { get; set; } = new();

    [JsonIgnore]
    public string FullName => $"{LastName} {FirstName} {MiddleName}";

    [JsonIgnore]
    public string Address => $"{City}, {Street}, {Building}" + (Apartment != null ? $", кв. {Apartment}" : "");
}
