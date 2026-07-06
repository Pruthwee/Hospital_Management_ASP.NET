using AutoMapper;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Mappings;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.UnitTests.Services;

/// <summary>Unit tests for PatientService.</summary>
public class PatientServiceTests
{
    private readonly Mock<IPatientRepository> _patientRepositoryMock;
    private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<PatientService>> _loggerMock;
    private readonly PatientService _sut;

    public PatientServiceTests()
    {
        _patientRepositoryMock = new Mock<IPatientRepository>();
        _appointmentRepositoryMock = new Mock<IAppointmentRepository>();
        _loggerMock = new Mock<ILogger<PatientService>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _sut = new PatientService(
            _patientRepositoryMock.Object,
            _appointmentRepositoryMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetPatientByIdAsync_WhenPatientExists_ReturnsPatientDto()
    {
        // Arrange
        var patient = new Patient
        {
            PatientId = 1,
            Name = "John Doe",
            Phone = "12345678901",
            Address = "123 Main St",
            BirthDate = new DateTime(1990, 1, 1),
            Gender = 'M'
        };
        _patientRepositoryMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(patient);

        // Act
        var result = await _sut.GetPatientByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.PatientId.Should().Be(1);
        result.Name.Should().Be("John Doe");
    }

    [Fact]
    public async Task GetPatientByIdAsync_WhenPatientNotFound_ReturnsNull()
    {
        // Arrange
        _patientRepositoryMock.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((Patient?)null);

        // Act
        var result = await _sut.GetPatientByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetBillHistoryAsync_ReturnsCorrectCount()
    {
        // Arrange
        var appointments = new List<Appointment>
        {
            new Appointment { AppointId = 1, PatientId = 1, BillStatus = "Paid", BillAmount = 100 },
            new Appointment { AppointId = 2, PatientId = 1, BillStatus = "Unpaid", BillAmount = 200 },
            new Appointment { AppointId = 3, PatientId = 1 } // No bill
        };
        _appointmentRepositoryMock.Setup(r => r.GetByPatientIdAsync(1, default)).ReturnsAsync(appointments);

        // Act
        var result = await _sut.GetBillHistoryAsync(1);

        // Assert
        result.Count.Should().Be(2);
        result.Bills.Should().HaveCount(2);
    }

    [Fact]
    public async Task SubmitFeedbackAsync_WhenAppointmentExists_ReturnsTrue()
    {
        // Arrange
        var appointment = new Appointment { AppointId = 1, FeedbackStatus = 2 };
        _appointmentRepositoryMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(appointment);
        _appointmentRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Appointment>(), default)).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.SubmitFeedbackAsync(1);

        // Assert
        result.Should().BeTrue();
        appointment.FeedbackStatus.Should().Be(1);
    }
}
