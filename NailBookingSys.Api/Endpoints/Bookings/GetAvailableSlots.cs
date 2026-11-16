using FastEndpoints;
using NailBookingSys.Contracts.Services;
using NailBookingSys.Contracts.Domain;

namespace NailBookingSys.Api.Endpoints.Bookings;

public class GetAvailableSlotsEndpoint : EndpointWithoutRequest<IEnumerable<DateTime>>
{
    private readonly IBookingService _booking;
    public GetAvailableSlotsEndpoint(IBookingService booking) => _booking = booking;

    public override void Configure()
    {
        Get("/api/appointments/available-slots");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var day = Query<DateTime>("day", isRequired: false);
        if (day == default) day = DateTime.Today;

        var serviceRaw = Query<string>("service", isRequired: false);
        if (string.IsNullOrWhiteSpace(serviceRaw) ||
            !Enum.TryParse<ServiceType>(serviceRaw, ignoreCase: true, out var service))
        {
            AddError("Query", "A valid 'service' is required.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var slots = await _booking.GetAvailableSlotsAsync(day, service, ct);
        await Send.OkAsync(slots, ct);
    }
}