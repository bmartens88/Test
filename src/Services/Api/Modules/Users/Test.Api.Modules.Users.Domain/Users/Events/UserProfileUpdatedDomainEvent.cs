using Test.Api.Common.Domain.Events;

namespace Test.Api.Modules.Users.Domain.Users.Events;

public record UserProfileUpdatedDomainEvent(Guid UserId, string Email, string DisplayName) : DomainEvent;