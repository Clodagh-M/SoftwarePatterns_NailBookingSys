using NailBookingSys.Contracts.Domain;
using NailBookingSys.Contracts.Events;
using NailBookingSys.Contracts.Exceptions;
using NailBookingSys.Contracts.Factories;
using NailBookingSys.Contracts.Persistence;

namespace NailBookingSys.Contracts.Services;

public sealed class BookingService : IBookingService
{
    private readonly IUnitOfWork _uow;
    private readonly INailServiceFactory _serviceFactory;
    private readonly IDomainEventPublisher _events;

    public BookingService(IUnitOfWork uow, INailServiceFactory serviceFactory, IDomainEventPublisher events)
    {
        _uow = uow;
        _serviceFactory = serviceFactory;
        _events = events;
    }

    public async Task<BookingResult> BookAsync(BookingRequest request, CancellationToken ct = default)
    {
        ValidateRequest(request);

        var service = _serviceFactory.Create(request.ServiceType);
        var end = request.StartTime.AddMinutes(service.DurationInMinutes);

        EnsureWithinOpeningHours(request.StartTime, end);
        EnsureNotInPast(request.StartTime);

        var staff = await _uow.Staff.GetByIdAsync(request.StaffId, ct)
            ?? throw new BookingException("The selected nail technician does not exist.");

        if (!staff.IsActive)
            throw new BookingException("The selected nail technician is not available.");

        await EnsureNoOverlapAsync(request.StaffId, request.StartTime, end, excludeId: null, ct);

        var appointment = new Appointment(
            Guid.NewGuid(), request.CustomerName.Trim(), request.StaffId,
            request.ServiceType, request.StartTime, end, service.Price);

        await _uow.Appointments.AddAsync(appointment, ct);
        await _uow.SaveChangesAsync(ct);

        await _events.PublishAsync(new AppointmentBookedEvent(appointment), ct);

        return new BookingResult(appointment.Id, appointment.StartTime, appointment.EndTime, appointment.Price);
    }

    public async Task<BookingResult> RescheduleAsync(Guid appointmentId, DateTime newStartTime, CancellationToken ct = default)
    {
        var appointment = await _uow.Appointments.GetByIdAsync(appointmentId, ct)
            ?? throw new BookingException("Appointment not found.");

        if (appointment.Status == AppointmentStatus.Cancelled)
            throw new BookingException("Cannot reschedule a cancelled appointment.");

        var service = _serviceFactory.Create(appointment.ServiceType);
        var newEnd = newStartTime.AddMinutes(service.DurationInMinutes);

        EnsureWithinOpeningHours(newStartTime, newEnd);
        EnsureNotInPast(newStartTime);
        await EnsureNoOverlapAsync(appointment.StaffId, newStartTime, newEnd, appointment.Id, ct);

        var oldStart = appointment.StartTime;
        var oldEnd = appointment.EndTime;
        appointment.Reschedule(newStartTime, newEnd);

        await _uow.Appointments.UpdateAsync(appointment, ct);
        await _uow.SaveChangesAsync(ct);

        await _events.PublishAsync(new AppointmentRescheduledEvent(appointment, oldStart, oldEnd), ct);

        return new BookingResult(appointment.Id, appointment.StartTime, appointment.EndTime, appointment.Price);
    }

    public async Task CancelAsync(Guid appointmentId, CancellationToken ct = default)
    {
        var appointment = await _uow.Appointments.GetByIdAsync(appointmentId, ct)
            ?? throw new BookingException("Appointment not found.");

        if (appointment.Status == AppointmentStatus.Cancelled)
            return;

        appointment.Cancel();
        await _uow.Appointments.UpdateAsync(appointment, ct);
        await _uow.SaveChangesAsync(ct);

        await _events.PublishAsync(new AppointmentCancelledEvent(appointment), ct);
    }

    public Task<IReadOnlyList<Appointment>> GetAppointmentsForDayAsync(DateTime day, CancellationToken ct = default)
        => _uow.Appointments.GetForDayAsync(day, ct);

    private static void ValidateRequest(BookingRequest r)
    {
        if (string.IsNullOrWhiteSpace(r.CustomerName))
            throw new BookingException("Please provide a customer name.");
        if (r.StaffId == Guid.Empty)
            throw new BookingException("Please select a nail technician.");
    }

    private static void EnsureNotInPast(DateTime start)
    {
        if (start < DateTime.Now)
            throw new BookingException("Appointments cannot be booked in the past.");
    }

    private static void EnsureWithinOpeningHours(DateTime start, DateTime end)
    {
        var startTime = start.TimeOfDay;
        var endTime = end.TimeOfDay;

        if (start.Date != end.Date)
            throw new BookingException("An appointment must start and end on the same day.");
        if (startTime < SalonSettings.OpeningTime || endTime > SalonSettings.ClosingTime)
            throw new BookingException(
                $"Appointments must fit within opening hours ({SalonSettings.OpeningTime:hh\\:mm}-{SalonSettings.ClosingTime:hh\\:mm}).");
    }

    private async Task EnsureNoOverlapAsync(Guid staffId, DateTime start, DateTime end, Guid? excludeId, CancellationToken ct)
    {
        var existing = await _uow.Appointments
            .GetActiveForStaffOnDayAsync(staffId, start.Date, ct);

        var conflict = existing.Any(a =>
            a.Id != excludeId &&
            a.StartTime < end && a.EndTime > start);

        if (conflict)
            throw new BookingException("This slot is already booked for the selected nail technician.");
    }

}
