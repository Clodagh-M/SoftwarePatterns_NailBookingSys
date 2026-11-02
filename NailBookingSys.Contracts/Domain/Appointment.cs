using System;
using System.Collections.Generic;
using System.Text;

namespace NailBookingSys.Contracts.Domain;

public class Appointment
{
    public Guid Id { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public Guid StaffId { get; private set; }
    public ServiceType ServiceType { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public decimal Price { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Appointment() { }

    public Appointment(string customerName, Guid staffId, ServiceType serviceType, DateTime startTime, DateTime endTime, decimal price)
    {
        Id = Guid.NewGuid();
        CustomerName = customerName;
        StaffId = staffId;
        ServiceType = serviceType;
        StartTime = startTime;
        EndTime = endTime;
        Price = price;
        Status = AppointmentStatus.Scheduled;
        CreatedAt = DateTime.UtcNow;
    }

    public void Cancel() => Status = AppointmentStatus.Cancelled;

    public void Reschedule(DateTime newStartTime, DateTime newEndTime)
    {
        StartTime = newStartTime;
        EndTime = newEndTime;
        Status = AppointmentStatus.Rescheduled;
    }
}

public enum AppointmentStatus
{
    Booked,
    Cancelled
}
