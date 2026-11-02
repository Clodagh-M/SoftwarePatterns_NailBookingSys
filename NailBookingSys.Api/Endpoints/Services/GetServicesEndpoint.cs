using FastEndpoints;
using NailBookingSys.Api.Contracts;
using NailBookingSys.Contracts.Factories;

namespace NailBookingSys.Api.Endpoints.Services;

public class GetServicesEndpoint : EndpointWithoutRequest<IEnumerable<ServiceDto>>
{
    private readonly INailServiceFactory _factory;
    public GetServicesEndpoint(INailServiceFactory factory) => _factory = factory;

    public override void Configure()
    {
        Get("/api/services");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var services = _factory.GetAll()
            .Select(s => new ServiceDto(s.ServiceType, s.Name, s.DurationInMinutes, s.Price));
        await Send.OkAsync(services, ct);
    }
}
