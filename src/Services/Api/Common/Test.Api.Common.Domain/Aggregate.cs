using Test.Api.Common.Domain.Events;

namespace Test.Api.Common.Domain;

public abstract class Aggregate<TId> : Entity<TId>, IAggregateRoot
    where TId : TypedId
{
    private static readonly List<IDomainEvent> DomainEvents = [];

    protected Aggregate(TId id)
        : base(id)
    {
    }

    protected Aggregate()
    {
    }

    public IReadOnlyList<IDomainEvent> PopDomainEvents()
    {
        var copy = DomainEvents.ToList();
        DomainEvents.Clear();

        return copy;
    }

    protected void Raise(IDomainEvent domainEvent)
    {
        DomainEvents.Add(domainEvent);
    }
}