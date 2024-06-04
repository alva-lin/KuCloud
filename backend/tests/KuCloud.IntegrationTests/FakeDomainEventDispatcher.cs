namespace KuCloud.IntegrationTests;

internal class FakeDomainEventDispatcher : IDomainEventDispatcher
{
    public Task DispatchAndClearEvents(IEnumerable<EntityBase> entitiesWithEvents)
    {
        return Task.CompletedTask;
    }

    public Task DispatchAndClearEvents<TId>(IEnumerable<EntityBase<TId>> entitiesWithEvents)
        where TId : struct, IEquatable<TId>
    {
        return Task.CompletedTask;
    }
}
