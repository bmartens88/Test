namespace Test.Api.Common.Domain.Events;

public abstract record DomainEvent(DateTime OccurredOnUtc) : IDomainEvent
{
    protected DomainEvent()
        : this(DateTime.UtcNow)
    {
    }
}