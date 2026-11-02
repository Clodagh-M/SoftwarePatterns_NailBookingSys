namespace NailBookingSys.Contracts.Domain.Services;

public sealed class ManicureService : INailService
{
    public ServiceType ServiceType => ServiceType.Manicure;
    public string Name => "Manicure";
    public int DurationInMinutes => 30;
    public decimal Price => 25.00m;
}
