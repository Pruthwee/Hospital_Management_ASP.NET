using ClinicManagement.Domain.Enums;

namespace ClinicManagement.Application.Interfaces.Services;

/// <summary>Service interface for authentication operations.</summary>
public interface IAuthService
{
    Task<(bool Success, UserType UserType, int UserId)> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<(bool Success, int UserId, string Message)> RegisterPatientAsync(string name, string birthDate, string email, string password, string phone, string gender, string address, CancellationToken cancellationToken = default);
}
