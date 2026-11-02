using FastEndpoints;
using NailBookingSys.Api.Contracts;
using NailBookingSys.Contracts.Services;

namespace NailBookingSys.Api.Endpoints.Bookings;

public class GetAppointmentsForDayEndpoint : EndpointWithoutRequest<IEnumerable<AppointmentDto>>
{
    private readonly IBookingService _booking;
    public GetAppointmentsForDayEndpoint(IBookingService booking) => _booking = booking;

    public override void Configure()
    {
        Get("/api/appointments");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var day = Query<DateTime>("day", isRequired: false);
        if (day == default) day = DateTime.Today;

        var appts = await _booking.GetAppointmentsForDayAsync(day, ct);
        await Send.OkAsync(appts.Select(a => new AppointmentDto(
            a.Id, a.CustomerName, a.StaffId, a.ServiceType,
            a.StartTime, a.EndTime, a.Price, a.Status.ToString())), ct);
    }
}
