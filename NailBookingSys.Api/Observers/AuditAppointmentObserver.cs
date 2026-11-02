using NailBookingSys.Contracts.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace NailBookingSys.Api.Observers;

public class AuditAppointmentObserver :
    IDomainEventHandler<AppointmentBookedEvent>,
    IDomainEventHandler<AppointmentCancelledEvent>,
    IDomainEventHandler<AppointmentRescheduledEvent>
{
    private static readonly ConcurrentQueue<string> _auditLog = new();
    public static IReadOnlyCollection<string> Log => _auditLog.ToArray();

    public Task HandleAsync(AppointmentBookedEvent e, CancellationToken ct = default)
        => Record($"BOOKED {e.Appointment.Id} at {e.Appointment.StartTime:O}");

    public Task HandleAsync(AppointmentCancelledEvent e, CancellationToken ct = default)
        => Record($"CANCELLED {e.Appointment.Id}");

    public Task HandleAsync(AppointmentRescheduledEvent e, CancellationToken ct = default)
        => Record($"RESCHEDULED {e.Appointment.Id} {e.OldStart:O}->{e.Appointment.StartTime:O}");

    private static Task Record(string msg) { _auditLog.Enqueue($"{DateTime.UtcNow:O} {msg}"); return Task.CompletedTask; }
}
