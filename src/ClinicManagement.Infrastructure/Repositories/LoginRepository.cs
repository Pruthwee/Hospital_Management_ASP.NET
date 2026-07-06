using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>EF Core implementation of ILoginRepository.</summary>
public class LoginRepository : ILoginRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<LoginRepository> _logger;

    public LoginRepository(ClinicDbContext context, ILogger<LoginRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<LoginTable?> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.LoginTable
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Email == email && l.Password == password, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating login for email: {Email}", email);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<LoginTable?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.LoginTable
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Email == email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting login by email: {Email}", email);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<LoginTable> AddAsync(LoginTable login, CancellationToken cancellationToken = default)
    {
        _context.LoginTable.Add(login);
        await _context.SaveChangesAsync(cancellationToken);
        return login;
    }

    /// <inheritdoc/>
    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.LoginTable.AnyAsync(l => l.Email == email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking email existence: {Email}", email);
            return false;
        }
    }
}
