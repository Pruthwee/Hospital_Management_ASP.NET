using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Infrastructure.Data;
using ClinicManagement.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.IntegrationTests.Repositories;

/// <summary>Integration tests for PatientRepository using in-memory database.</summary>
public class PatientRepositoryTests : IDisposable
{
    private readonly ClinicDbContext _context;
    private readonly PatientRepository _sut;

    public PatientRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ClinicDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ClinicDbContext(options);
        var loggerMock = new Mock<ILogger<PatientRepository>>();
        _sut = new PatientRepository(_context, loggerMock.Object);

        SeedData();
    }

    private void SeedData()
    {
        var login = new LoginTable { LoginId = 1, Email = "patient@test.com", Password = "pass", Type = UserType.Patient };
        _context.LoginTable.Add(login);

        var patient = new Patient
        {
            PatientId = 1,
            Name = "Test Patient",
            Phone = "12345678901",
            Address = "Test Address",
            BirthDate = new DateTime(1990, 5, 15),
            Gender = 'M'
        };
        _context.Patients.Add(patient);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetByIdAsync_WhenPatientExists_ReturnsPatient()
    {
        // Act
        var result = await _sut.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Test Patient");
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllPatients()
    {
        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingQuery_ReturnsMatchingPatients()
    {
        // Act
        var result = await _sut.SearchAsync("Test");

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Contain("Test");
    }

    [Fact]
    public async Task ExistsAsync_WhenPatientExists_ReturnsTrue()
    {
        // Act
        var result = await _sut.ExistsAsync(1);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WhenPatientNotExists_ReturnsFalse()
    {
        // Act
        var result = await _sut.ExistsAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
