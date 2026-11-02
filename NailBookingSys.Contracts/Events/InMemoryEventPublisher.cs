using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace NailBookingSys.Contracts.Events;

public sealed class InMemoryEventPublisher : IDomainEventPublisher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InMemoryEventPublisher> _logger;

    public InMemoryEventPublisher(IServiceProvider serviceProvider, ILogger<InMemoryEventPublisher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct = default)
    {
        var handlers = _serviceProvider.GetServices<IDomainEventHandler<TEvent>>();
        foreach (var handler in handlers)
        {
            try
            {
                await handler.HandleAsync(@event, ct);
            }
            catch (Exception ex)
            {
                // Observers must not break the caller — log and continue
                _logger.LogError(ex, "Handler {Handler} failed for {Event}",
                    handler.GetType().Name, typeof(TEvent).Name);
            }
        }
    }
}
