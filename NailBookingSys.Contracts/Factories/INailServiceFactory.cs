using NailBookingSys.Contracts.Domain;

namespace NailBookingSys.Contracts.Factories;

public interface INailServiceFactory
{
    INailService Create(ServiceType type);
    IEnumerable<INailService> GetAll();
}
