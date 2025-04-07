using Test.Api.Common.Domain;

namespace Test.Api.Modules.Users.Domain.Users.ValueObjects;

public sealed class UserId : TypedId<Guid>
{
    private UserId(Guid value)
        : base(value)
    {
    }

    public static UserId Create(Guid value)
    {
        return new UserId(value);
    }

    public static UserId CreateNew()
    {
        return new UserId(Guid.NewGuid());
    }
}