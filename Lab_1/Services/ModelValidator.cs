using System.ComponentModel.DataAnnotations;

namespace Lab_1.Services;

public static class ModelValidator
{
    public static List<string> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);
        return results.Select(r => r.ErrorMessage ?? "Некоректне значення").ToList();
    }
}
