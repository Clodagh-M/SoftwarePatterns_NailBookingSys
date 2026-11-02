namespace NailBookingSys.Contracts.Domain.Services;

public sealed class PedicureService : INailService
{
    public ServiceType ServiceType => ServiceType.Pedicure;
    public string Name => "Pedicure";
    public int DurationInMinutes => 60;
    public decimal Price => 35.00m;
}
