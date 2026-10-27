using System.ComponentModel.DataAnnotations;
using Grpc.Core;

public static class ModelValidator
{
    public static List<string> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);
        return results.Select(r => r.ErrorMessage ?? "Некоректне значення").ToList();
    }

    public static void ThrowIfInvalid(List<string> errors)
    {
        if (errors.Count > 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, string.Join("; ", errors)));
    }

    public static RpcException NotFound(string message)
    {
        return new RpcException(new Status(StatusCode.NotFound, message));
    }
}
