namespace NailBookingSys.Contracts.Persistence;

public interface IUnitOfWork
{
    IAppointmentRepository Appointments { get; }
    IStaffRepository Staff { get; }
    Task SaveChangesAsync(CancellationToken ct = default);
}
