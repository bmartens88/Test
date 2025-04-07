using Ardalis.GuardClauses;

namespace Test.Api.Common.Domain;

public abstract class TypedId<TValue>(TValue value) : TypedId
    where TValue : struct
{
    public TValue Value { get; } = Guard.Against.Default(value);

    protected sealed override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

public abstract class TypedId : ValueObject;