using NailBookingSys.Contracts.Domain;

namespace NailBookingSys.Contracts.Persistence;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Appointment>> GetForDayAsync(DateTime day, CancellationToken ct = default);
    Task<IReadOnlyList<Appointment>> GetActiveForStaffOnDayAsync(Guid staffId, DateTime day, CancellationToken ct = default);
    Task AddAsync(Appointment appointment, CancellationToken ct = default);
    Task UpdateAsync(Appointment appointment, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
