using FastEndpoints;
using NailBookingSys.Api.Contracts;
using NailBookingSys.Contracts.Exceptions;
using NailBookingSys.Contracts.Services;

namespace NailBookingSys.Api.Endpoints.Bookings;

public record RescheduleRequestDto(DateTime NewStartTime);

public class RescheduleAppointmentEndpoint : Endpoint<RescheduleRequestDto, BookAppointmentResponseDto>
{
    private readonly IBookingService _booking;
    public RescheduleAppointmentEndpoint(IBookingService booking) => _booking = booking;

    public override void Configure()
    {
        Post("/api/appointments/{id}/reschedule");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RescheduleRequestDto req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        try
        {
            var r = await _booking.RescheduleAsync(id, req.NewStartTime, ct);
            await Send.OkAsync(new BookAppointmentResponseDto(r.AppointmentId, r.StartTime, r.EndTime, r.Price), ct);
        }
        catch (BookingException ex)
        {
            AddError(ex.Message);
            await Send.ErrorsAsync(400, ct);
        }
    }
}
