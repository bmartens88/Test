namespace Test.Api.Common.Domain.Errors;

public record ValidationError(Error[] Errors) : Error(
    "General.ValidationError",
    "One ore more validation errors occurred",
    ErrorType.Validation)
{
    public static ValidationError FromResults(IEnumerable<Result> results)
    {
        return new ValidationError(results.Where(result => result.IsFailure).Select(result => result.Error).ToArray());
    }
}