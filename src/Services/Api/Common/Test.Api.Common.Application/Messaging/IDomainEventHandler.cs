using Test.Api.Common.Domain.Events;

namespace Test.Api.Common.Application.Messaging;

public interface IDomainEventHandler<in TDomainEvent> : IDomainEventHandler
    where TDomainEvent : IDomainEvent
{
    Task Handle(TDomainEvent domainEvent, CancellationToken cancellationToken = default);
}

public interface IDomainEventHandler
{
    Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}