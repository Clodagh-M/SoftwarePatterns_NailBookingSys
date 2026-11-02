using Microsoft.EntityFrameworkCore;
using NailBookingSys.Contracts.Domain;
using NailBookingSys.Contracts.Persistence;

namespace NailBookingSys.Api.Persistence;

public class StaffRepository : IStaffRepository
{
    private readonly SalonDbContext _db;
    public StaffRepository(SalonDbContext db) => _db = db;

    public async Task<IReadOnlyList<StaffMember>> GetAllActiveAsync(CancellationToken ct = default) =>
        await _db.Staff.Where(s => s.IsActive).ToListAsync(ct);

    public Task<StaffMember?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Staff.FirstOrDefaultAsync(s => s.Id == id, ct);
}
