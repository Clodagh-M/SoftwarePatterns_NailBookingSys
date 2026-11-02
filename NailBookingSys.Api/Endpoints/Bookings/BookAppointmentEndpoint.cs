using FastEndpoints;
using NailBookingSys.Api.Contracts;
using NailBookingSys.Contracts.Domain;
using NailBookingSys.Contracts.Exceptions;
using NailBookingSys.Contracts.Services;

namespace NailBookingSys.Api.Endpoints.Bookings;

public class BookAppointmentEndpoint : Endpoint<BookAppointmentRequestDto, BookAppointmentResponseDto>
{
    private readonly IBookingService _booking;

    public BookAppointmentEndpoint(IBookingService booking) => _booking = booking;

    public override void Configure()
    {
        Post("/api/appointments");
        AllowAnonymous();
    }

    public override async Task HandleAsync(BookAppointmentRequestDto req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.CustomerName) || req.CustomerName.Length > 150)
        {
            AddError(r => r.CustomerName, "Please enter a customer name (max 150 chars).");
            await Send.ErrorsAsync(400, ct);
            return;
        }
        if (!Enum.IsDefined(typeof(ServiceType), req.ServiceType))
        {
            AddError(r => r.ServiceType, "Unknown service type.");
            await Send.ErrorsAsync(400, ct);
            return;
        }
        if (req.StaffId == Guid.Empty)
        {
            AddError(r => r.StaffId, "Please select a nail technician.");
            await Send.ErrorsAsync(400, ct);
            return;
        }

        try
        {
            var result = await _booking.BookAsync(
                new BookingRequest(req.CustomerName, req.StaffId, req.ServiceType, req.StartTime), ct);

            await Send.CreatedAtAsync<BookAppointmentEndpoint>(
                new { id = result.AppointmentId },
                new BookAppointmentResponseDto(result.AppointmentId, result.StartTime, result.EndTime, result.Price),
                cancellation: ct);
        }
        catch (BookingException ex)
        {
            AddError(ex.Message);
            await Send.ErrorsAsync(400, ct);
        }
    }
}
