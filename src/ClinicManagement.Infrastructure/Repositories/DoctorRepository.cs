using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>EF Core implementation of IDoctorRepository.</summary>
public class DoctorRepository : IDoctorRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<DoctorRepository> _logger;

    public DoctorRepository(ClinicDbContext context, ILogger<DoctorRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Doctor?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Doctors
                .AsNoTracking()
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.DoctorId == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctor by ID: {Id}", id);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Doctor>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Doctors
                .AsNoTracking()
                .Include(d => d.Department)
                .Where(d => d.Status == DoctorStatus.Present)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all active doctors");
            return Enumerable.Empty<Doctor>();
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Doctor>> SearchAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Doctors
                .AsNoTracking()
                .Include(d => d.Department)
                .Where(d => d.Status == DoctorStatus.Present && d.Name.Contains(searchQuery))
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching doctors with query: {Query}", searchQuery);
            return Enumerable.Empty<Doctor>();
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Doctor>> GetByDepartmentAsync(string departmentName, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Doctors
                .AsNoTracking()
                .Include(d => d.Department)
                .Where(d => d.Status == DoctorStatus.Present && d.Department != null && d.Department.DeptName == departmentName)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctors by department: {Department}", departmentName);
            return Enumerable.Empty<Doctor>();
        }
    }

    /// <inheritdoc/>
    public async Task AddAsync(Doctor doctor, CancellationToken cancellationToken = default)
    {
        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(Doctor doctor, CancellationToken cancellationToken = default)
    {
        _context.Doctors.Update(doctor);
        await _context.SaveChangesAsync(cancellationToken);
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
            _logger.LogError(ex, "Error checking doctor email existence: {Email}", email);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Doctors.AnyAsync(d => d.DoctorId == id, cancellationToken);
    }
}
