namespace NailBookingSys.Contracts.Domain;

public class StaffMember
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
