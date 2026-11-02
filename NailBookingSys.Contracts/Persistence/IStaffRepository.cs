using NailBookingSys.Contracts.Domain;

namespace NailBookingSys.Contracts.Persistence;

public interface IStaffRepository
{
    Task<IReadOnlyList<StaffMember>> GetAllActiveAsync(CancellationToken ct = default);
    Task<StaffMember?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
