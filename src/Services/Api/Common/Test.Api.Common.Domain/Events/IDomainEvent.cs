using MediatR;

namespace Test.Api.Common.Domain.Events;

public interface IDomainEvent : INotification
{
    DateTime OccurredOnUtc { get; }
}