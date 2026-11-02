using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using NailBookingSys.Api.Observers;
using NailBookingSys.Api.Persistence;
using NailBookingSys.Contracts.Events;
using NailBookingSys.Contracts.Factories;
using NailBookingSys.Contracts.Persistence;
using NailBookingSys.Contracts.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SalonDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? "Data Source=salon.db"));

builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddSingleton<INailServiceFactory, NailServiceFactory>();
builder.Services.AddScoped<IBookingService, BookingService>();

builder.Services.AddScoped<IDomainEventPublisher, InMemoryEventPublisher>();
builder.Services.AddScoped<IDomainEventHandler<AppointmentBookedEvent>, LoggingAppointmentObserver>();
builder.Services.AddScoped<IDomainEventHandler<AppointmentCancelledEvent>, LoggingAppointmentObserver>();
builder.Services.AddScoped<IDomainEventHandler<AppointmentRescheduledEvent>, LoggingAppointmentObserver>();
builder.Services.AddScoped<IDomainEventHandler<AppointmentBookedEvent>, AuditAppointmentObserver>();
builder.Services.AddScoped<IDomainEventHandler<AppointmentCancelledEvent>, AuditAppointmentObserver>();
builder.Services.AddScoped<IDomainEventHandler<AppointmentRescheduledEvent>, AuditAppointmentObserver>();

builder.Services.AddFastEndpoints();

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseCors();
app.UseFastEndpoints();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SalonDbContext>();
    db.Database.Migrate();
    if (!db.Staff.Any())
    {
        db.Staff.AddRange(
            new NailBookingSys.Contracts.Domain.StaffMember { Id = Guid.NewGuid(), Name = "Clodagh Mulvihill" },
            new NailBookingSys.Contracts.Domain.StaffMember { Id = Guid.NewGuid(), Name = "Wiktoria Depta" },
            new NailBookingSys.Contracts.Domain.StaffMember { Id = Guid.NewGuid(), Name = "Karen Amumpaire" });
        db.SaveChanges();
    }
}

app.Run();