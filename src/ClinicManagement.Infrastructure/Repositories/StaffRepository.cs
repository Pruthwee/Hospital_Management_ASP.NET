using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>EF Core implementation of IStaffRepository.</summary>
public class StaffRepository : IStaffRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<StaffRepository> _logger;

    public StaffRepository(ClinicDbContext context, ILogger<StaffRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<OtherStaff?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.OtherStaff.AsNoTracking()
                .FirstOrDefaultAsync(s => s.StaffId == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting staff by ID: {Id}", id);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<OtherStaff>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.OtherStaff.AsNoTracking().ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all staff");
            return Enumerable.Empty<OtherStaff>();
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<OtherStaff>> SearchAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.OtherStaff.AsNoTracking()
                .Where(s => s.Name.Contains(searchQuery))
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching staff with query: {Query}", searchQuery);
            return Enumerable.Empty<OtherStaff>();
        }
    }

    /// <inheritdoc/>
    public async Task AddAsync(OtherStaff staff, CancellationToken cancellationToken = default)
    {
        _context.OtherStaff.Add(staff);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var staff = await _context.OtherStaff.FindAsync(new object[] { id }, cancellationToken);
        if (staff != null)
        {
            _context.OtherStaff.Remove(staff);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.OtherStaff.AnyAsync(s => s.StaffId == id, cancellationToken);
    }
}
