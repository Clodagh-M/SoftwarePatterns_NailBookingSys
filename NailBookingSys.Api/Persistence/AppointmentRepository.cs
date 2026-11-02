using Microsoft.EntityFrameworkCore;
using NailBookingSys.Contracts.Domain;
using NailBookingSys.Contracts.Persistence;

namespace NailBookingSys.Api.Persistence;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly SalonDbContext _db;
    public AppointmentRepository(SalonDbContext db) => _db = db;

    public Task<Appointment?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Appointments.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<IReadOnlyList<Appointment>> GetForDayAsync(DateTime day, CancellationToken ct = default)
    {
        var start = day.Date;
        var end = start.AddDays(1);
        return await _db.Appointments
            .Where(a => a.StartTime >= start && a.StartTime < end)
            .OrderBy(a => a.StartTime)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Appointment>> GetActiveForStaffOnDayAsync(Guid staffId, DateTime day, CancellationToken ct = default)
    {
        var start = day.Date;
        var end = start.AddDays(1);
        return await _db.Appointments
            .Where(a => a.StaffId == staffId
                     && a.Status == AppointmentStatus.Booked
                     && a.StartTime >= start && a.StartTime < end)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Appointment appointment, CancellationToken ct = default) =>
        await _db.Appointments.AddAsync(appointment, ct);

    public Task UpdateAsync(Appointment appointment, CancellationToken ct = default)
    {
        _db.Appointments.Update(appointment);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
