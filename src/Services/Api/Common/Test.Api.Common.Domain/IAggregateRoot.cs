using Test.Api.Common.Domain.Events;

namespace Test.Api.Common.Domain;

public interface IAggregateRoot
{
    IReadOnlyList<IDomainEvent> PopDomainEvents();
}