using AutoMapper;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>Service implementation for authentication operations.</summary>
public class AuthService : IAuthService
{
    private readonly ILoginRepository _loginRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        ILoginRepository loginRepository,
        IPatientRepository patientRepository,
        ILogger<AuthService> logger)
    {
        _loginRepository = loginRepository;
        _patientRepository = patientRepository;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<(bool Success, UserType UserType, int UserId)> ValidateLoginAsync(
        string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating login for email: {Email}", email);
            var login = await _loginRepository.ValidateLoginAsync(email, password, cancellationToken);
            if (login == null)
            {
                _logger.LogWarning("Login failed for email: {Email}", email);
                return (false, UserType.Patient, 0);
            }
            _logger.LogInformation("Login successful for email: {Email}, Type: {Type}", email, login.Type);
            return (true, login.Type, login.LoginId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating login for email: {Email}", email);
            return (false, UserType.Patient, 0);
        }
    }

    /// <inheritdoc/>
    public async Task<(bool Success, int UserId, string Message)> RegisterPatientAsync(
        string name, string birthDate, string email, string password,
        string phone, string gender, string address, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Registering new patient with email: {Email}", email);

            if (await _loginRepository.EmailExistsAsync(email, cancellationToken))
            {
                return (false, 0, "Email already exists.");
            }

            var login = new LoginTable
            {
                Email = email,
                Password = password,
                Type = UserType.Patient
            };

            var createdLogin = await _loginRepository.AddAsync(login, cancellationToken);

            var patient = new Patient
            {
                PatientId = createdLogin.LoginId,
                Name = name,
                Phone = phone,
                Address = address,
                BirthDate = DateTime.Parse(birthDate),
                Gender = gender.Length > 0 ? gender[0] : 'M'
            };

            await _patientRepository.AddAsync(patient, cancellationToken);
            _logger.LogInformation("Patient registered successfully with ID: {Id}", createdLogin.LoginId);
            return (true, createdLogin.LoginId, "Registration successful.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering patient with email: {Email}", email);
            return (false, 0, "Registration failed due to an error.");
        }
    }
}
