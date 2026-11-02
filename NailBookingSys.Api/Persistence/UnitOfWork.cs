using NailBookingSys.Contracts.Persistence;

namespace NailBookingSys.Api.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly SalonDbContext _db;
    public UnitOfWork(SalonDbContext db)
    {
        _db = db;
        Appointments = new AppointmentRepository(db);
        Staff = new StaffRepository(db);
    }

    public IAppointmentRepository Appointments { get; }
    public IStaffRepository Staff { get; }

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
