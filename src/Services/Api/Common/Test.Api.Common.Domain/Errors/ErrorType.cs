using Test.Api.Common.Domain.Enums;

namespace Test.Api.Common.Domain.Errors;

public record ErrorType : SmartEnum<ErrorType>
{
    public static readonly ErrorType Failure = new(nameof(Failure), 0);
    public static readonly ErrorType Validation = new(nameof(Validation), 1);
    public static readonly ErrorType Problem = new(nameof(Problem), 2);
    public static readonly ErrorType NotFound = new(nameof(NotFound), 3);
    public static readonly ErrorType Conflict = new(nameof(Conflict), 4);

    private ErrorType(string name, int value) : base(name, value)
    {
    }
}