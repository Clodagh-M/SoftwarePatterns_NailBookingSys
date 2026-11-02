using FastEndpoints;
using NailBookingSys.Contracts.Exceptions;
using NailBookingSys.Contracts.Services;

namespace NailBookingSys.Api.Endpoints.Bookings;

public class CancelAppointmentEndpoint : EndpointWithoutRequest
{
    private readonly IBookingService _booking;
    public CancelAppointmentEndpoint(IBookingService booking) => _booking = booking;

    public override void Configure()
    {
        Post("/api/appointments/{id}/cancel");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        try
        {
            await _booking.CancelAsync(id, ct);
            await Send.NoContentAsync(ct);
        }
        catch (BookingException ex)
        {
            AddError(ex.Message);
            await Send.ErrorsAsync(400, ct);
        }
    }
}
