using FastEndpoints;
using NailBookingSys.Api.Contracts;
using NailBookingSys.Contracts.Persistence;

namespace NailBookingSys.Api.Endpoints.Staff;

public class GetStaffEndpoint : EndpointWithoutRequest<IEnumerable<StaffDto>>
{
    private readonly IUnitOfWork _uow;
    public GetStaffEndpoint(IUnitOfWork uow) => _uow = uow;

    public override void Configure()
    {
        Get("/api/staff");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var staff = await _uow.Staff.GetAllActiveAsync(ct);
        await Send.OkAsync(staff.Select(s => new StaffDto(s.Id, s.Name)), ct);
    }
}
