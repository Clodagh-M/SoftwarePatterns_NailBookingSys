using NailBookingSys.Contracts.Domain;

namespace NailBookingSys.Api.Contracts;

public record BookAppointmentRequestDto(
    string CustomerName,
    Guid StaffId,
    ServiceType ServiceType,
    DateTime StartTime);

public record BookAppointmentResponseDto(
    Guid AppointmentId, DateTime StartTime, DateTime EndTime, decimal Price);

public record AppointmentDto(
    Guid Id, string CustomerName, Guid StaffId, ServiceType ServiceType,
    DateTime StartTime, DateTime EndTime, decimal Price, string Status);

public record StaffDto(Guid Id, string Name);

public record ServiceDto(ServiceType Type, string Name, int DurationMinutes, decimal Price);
