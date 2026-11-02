using NailBookingSys.Contracts.Domain;

namespace NailBookingSys.Contracts.Services;

public record BookingResult(Guid AppointmentId, DateTime StartTime, DateTime EndTime, decimal Price);

public record BookingRequest(
    string CustomerName,
    Guid StaffId,
    ServiceType ServiceType,
    DateTime StartTime);

public interface IBookingService
{
    Task<BookingResult> BookAsync(BookingRequest request, CancellationToken ct = default);
    Task<BookingResult> RescheduleAsync(Guid appointmentId, DateTime newStartTime, CancellationToken ct = default);
    Task CancelAsync(Guid appointmentId, CancellationToken ct = default);
    Task<IReadOnlyList<Appointment>> GetAppointmentsForDayAsync(DateTime day, CancellationToken ct = default);
}
