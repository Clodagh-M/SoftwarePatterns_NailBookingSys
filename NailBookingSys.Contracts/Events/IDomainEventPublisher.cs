namespace NailBookingSys.Contracts.Events;

public interface IDomainEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct = default);
}
