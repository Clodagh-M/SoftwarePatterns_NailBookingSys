using NailBookingSys.Contracts.Domain;
using NailBookingSys.Contracts.Domain.Services;

namespace NailBookingSys.Contracts.Factories;

public sealed class NailServiceFactory : INailServiceFactory
{
    private readonly Dictionary<ServiceType, Func<INailService>> _registry = new()
    {
        [ServiceType.Manicure] = () => new ManicureService(),
        [ServiceType.Pedicure] = () => new PedicureService(),
        [ServiceType.GelExtensions] = () => new GelExtensionsService(),
        [ServiceType.NailArt] = () => new NailArtService()
    };

    public INailService Create(ServiceType type)
    {
        if (!_registry.TryGetValue(type, out var factory))
            throw new ArgumentException($"Unknown service type: {type}", nameof(type));

        return factory();
    }

    public IEnumerable<INailService> GetAll() =>
        _registry.Values.Select(f => f());
}

