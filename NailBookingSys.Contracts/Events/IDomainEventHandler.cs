namespace NailBookingSys.Contracts.Events;

public interface IDomainEventHandler<in TEvent>
{
    Task HandleAsync(TEvent @event, CancellationToken ct = default);
}

