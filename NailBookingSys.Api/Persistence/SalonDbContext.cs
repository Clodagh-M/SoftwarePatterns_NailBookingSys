using Microsoft.EntityFrameworkCore;
using NailBookingSys.Contracts.Domain;

namespace NailBookingSys.Api.Persistence;

public class SalonDbContext : DbContext
{
    public SalonDbContext(DbContextOptions<SalonDbContext> options) : base(options) { }

    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<StaffMember> Staff => Set<StaffMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.CustomerName).IsRequired().HasMaxLength(150);
            e.Property(a => a.Status).HasConversion<string>();

            e.HasIndex(a => new { a.StaffId, a.StartTime }).IsUnique();
        });

        modelBuilder.Entity<StaffMember>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.Name).IsRequired().HasMaxLength(100);
        });
    }
}
