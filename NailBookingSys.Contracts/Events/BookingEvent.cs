using NailBookingSys.Contracts.Domain;

namespace NailBookingSys.Contracts.Events;

public record AppointmentBookedEvent(Appointment Appointment);
public record AppointmentCancelledEvent(Appointment Appointment);
public record AppointmentRescheduledEvent(Appointment Appointment, DateTime OldStart, DateTime OldEnd);
