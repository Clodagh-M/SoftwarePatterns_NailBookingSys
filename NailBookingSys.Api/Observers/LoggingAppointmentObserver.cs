using Microsoft.Extensions.Logging;
using NailBookingSys.Contracts.Events;

namespace NailBookingSys.Api.Observers;

public class LoggingAppointmentObserver :
    IDomainEventHandler<AppointmentBookedEvent>,
    IDomainEventHandler<AppointmentCancelledEvent>,
    IDomainEventHandler<AppointmentRescheduledEvent>
{
    private readonly ILogger<LoggingAppointmentObserver> _log;
    public LoggingAppointmentObserver(ILogger<LoggingAppointmentObserver> log) => _log = log;

    public Task HandleAsync(AppointmentBookedEvent e, CancellationToken ct = default)
    {
        _log.LogInformation("Booked {Id} for {Customer} at {Start}", e.Appointment.Id, e.Appointment.CustomerName, e.Appointment.StartTime);
        return Task.CompletedTask;
    }

    public Task HandleAsync(AppointmentCancelledEvent e, CancellationToken ct = default)
    {
        _log.LogInformation("Cancelled {Id}", e.Appointment.Id);
        return Task.CompletedTask;
    }

    public Task HandleAsync(AppointmentRescheduledEvent e, CancellationToken ct = default)
    {
        _log.LogInformation("Rescheduled {Id} {Old} -> {New}", e.Appointment.Id, e.OldStart, e.Appointment.StartTime);
        return Task.CompletedTask;
    }
}
