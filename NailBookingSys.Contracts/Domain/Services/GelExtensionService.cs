namespace NailBookingSys.Contracts.Domain.Services;

public sealed class GelExtensionsService : INailService
{
    public ServiceType ServiceType => ServiceType.GelExtensions;
    public string Name => "Gel Extensions";
    public int DurationInMinutes => 90;
    public decimal Price => 60.00m;
}