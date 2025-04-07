using Ardalis.GuardClauses;
using Test.Api.Common.Domain;
using Test.Api.Common.Domain.Guards;
using Test.Api.Modules.Users.Domain.Users.Events;
using Test.Api.Modules.Users.Domain.Users.ValueObjects;

namespace Test.Api.Modules.Users.Domain.Users;

public sealed class User : Aggregate<UserId>
{
    private User(
        string email,
        string displayName,
        UserId id) : base(id)
    {
        Email = email;
        DisplayName = displayName;
    }

    public string Email { get; private set; }

    public string DisplayName { get; private set; }

    public static User Create(
        string email,
        string displayName,
        UserId? id = null)
    {
        Guard.Against.Length(Guard.Against.NullOrEmpty(email), 100, nameof(email));
        Guard.Against.Length(Guard.Against.NullOrEmpty(displayName), 100, nameof(displayName));

        var user = new User(email, displayName, id ?? UserId.CreateNew());
        user.Raise(new UserRegisteredDomainEvent(user.Id.Value));

        return user;
    }
}