
namespace NailBookingSys.Contracts.Domain.Services;

public sealed class NailArtService : INailService
{
    public ServiceType ServiceType => ServiceType.NailArt;
    public string Name => "Nail Art";
    public int DurationInMinutes => 75;
    public decimal Price => 45.00m;
}