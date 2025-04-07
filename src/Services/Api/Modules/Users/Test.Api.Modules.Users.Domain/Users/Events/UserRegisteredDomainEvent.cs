using Test.Api.Common.Domain.Events;

namespace Test.Api.Modules.Users.Domain.Users.Events;

public record UserRegisteredDomainEvent(Guid UserId) : DomainEvent;