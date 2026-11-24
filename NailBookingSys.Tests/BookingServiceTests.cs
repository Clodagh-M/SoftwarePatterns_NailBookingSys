using NSubstitute;
using NailBookingSys.Contracts.Domain;
using NailBookingSys.Contracts.Events;
using NailBookingSys.Contracts.Exceptions;
using NailBookingSys.Contracts.Factories;
using NailBookingSys.Contracts.Persistence;
using NailBookingSys.Contracts.Services;
using Xunit;

namespace NailBookingSys.Tests;

public class BookingServiceTests
{
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly IAppointmentRepository _appointments = Substitute.For<IAppointmentRepository>();
    private readonly IStaffRepository _staff = Substitute.For<IStaffRepository>();
    private readonly IDomainEventPublisher _events = Substitute.For<IDomainEventPublisher>();

    private readonly INailServiceFactory _factory = new NailServiceFactory();

    private readonly BookingService _testBookingService;
    private readonly Guid _staffId = Guid.NewGuid();

    public BookingServiceTests()
    {
        _uow.Appointments.Returns(_appointments);
        _uow.Staff.Returns(_staff);

        _staff.GetByIdAsync(_staffId, Arg.Any<CancellationToken>())
              .Returns(new StaffMember { Id = _staffId, Name = "Clodagh", IsActive = true });

        _appointments.GetActiveForStaffOnDayAsync(
                _staffId, Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(Array.Empty<Appointment>());

        _testBookingService = new BookingService(_uow, _factory, _events);
    }

    private static DateTime TomorrowAt(int hour, int minute = 0)
        => DateTime.Today.AddDays(1).AddHours(hour).AddMinutes(minute);

    [Fact]
    public async Task BookAsync_ValidManicure_ReturnsCorrectEndTimeAndPrice()
    {
        // Arrange
        var start = TomorrowAt(10);
        var request = new BookingRequest("Clodagh", _staffId, ServiceType.Manicure, start);

        // Act
        var result = await _testBookingService.BookAsync(request);

        // Assert
        Assert.NotEqual(Guid.Empty, result.AppointmentId);
        Assert.Equal(start, result.StartTime);
        Assert.Equal(start.AddMinutes(30), result.EndTime);  // Manicure = 30 min
        Assert.Equal(25.00m, result.Price);
    }

    [Fact]
    public async Task BookAsync_BeforeOpeningHours_ThrowsBookingException()
    {
        // Arrange — 08:00 is before the 09:00 opening time
        var start = TomorrowAt(8);
        var request = new BookingRequest("Clodagh", _staffId, ServiceType.Manicure, start);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BookingException>(() => _testBookingService.BookAsync(request));
        Assert.Contains("opening hours", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task BookAsync_OverlappingExistingAppointment_ThrowsBookingException()
    {
        // Arrange — Clodagh already has a 10:00–11:00 pedicure
        var start = TomorrowAt(10);
        _appointments
            .GetActiveForStaffOnDayAsync(_staffId, start.Date, Arg.Any<CancellationToken>())
            .Returns(new[]
            {
            new Appointment(Guid.NewGuid(), "Wiktoria", _staffId,
                ServiceType.Pedicure, start, start.AddMinutes(60), 35m)
            });

        // Try to book a manicure at 10:30 — overlaps
        var request = new BookingRequest("Clodagh", _staffId, ServiceType.Manicure,
            start.AddMinutes(30));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BookingException>(() => _testBookingService.BookAsync(request));
        Assert.Contains("already booked", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetAppointmentsForDay_ReturnsRepositoryResults()
    {
        // Arrange
        var day = TomorrowAt(10).Date;
        var stored = new[]
        {
        new Appointment(Guid.NewGuid(), "Clodagh", _staffId,
            ServiceType.Manicure, TomorrowAt(10), TomorrowAt(10).AddMinutes(45), 25m)
    };
        _appointments.GetForDayAsync(day, Arg.Any<CancellationToken>())
                     .Returns(stored);

        // Act
        var result = await _testBookingService.GetAppointmentsForDayAsync(day);

        // Assert
        Assert.Single(result);
        Assert.Equal("Clodagh", result[0].CustomerName);
    }

    [Fact]
    public async Task BookAsync_Success_PublishesBookedEventOnce()
    {
        // Arrange
        var request = new BookingRequest("Clodagh", _staffId, ServiceType.Manicure, TomorrowAt(11));

        // Act
        await _testBookingService.BookAsync(request);

        // Assert
        await _events.Received(1).PublishAsync(
            Arg.Any<AppointmentBookedEvent>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RescheduleAsync_CancelledAppointment_ThrowsBookingException()
    {
        // Arrange — an appointment that is already cancelled
        var appointmentId = Guid.NewGuid();
        var cancelled = new Appointment(appointmentId, "Clodagh", _staffId,
            ServiceType.Manicure, TomorrowAt(10), TomorrowAt(10).AddMinutes(45), 25m);
        cancelled.Cancel();

        _appointments.GetByIdAsync(appointmentId, Arg.Any<CancellationToken>())
                     .Returns(cancelled);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BookingException>(
            () => _testBookingService.RescheduleAsync(appointmentId, TomorrowAt(14)));
        Assert.Contains("cancelled", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}

