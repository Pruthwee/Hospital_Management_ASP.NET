using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>EF Core implementation of IAppointmentRepository.</summary>
public class AppointmentRepository : IAppointmentRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<AppointmentRepository> _logger;

    public AppointmentRepository(ClinicDbContext context, ILogger<AppointmentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Appointment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointId == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting appointment by ID: {Id}", id);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting appointments for patient: {PatientId}", patientId);
            return Enumerable.Empty<Appointment>();
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting appointments for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Appointment>> GetPendingByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId && a.AppointmentStatus == AppointmentStatus.Pending)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending appointments for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Appointment>> GetTodaysByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var today = DateTime.Today;
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId
                    && a.AppointmentStatus == AppointmentStatus.Approved
                    && a.Date.HasValue
                    && a.Date.Value.Date == today)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting today's appointments for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }

    /// <inheritdoc/>
    public async Task<Appointment?> GetCurrentByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var today = DateTime.Today;
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.PatientId == patientId
                    && a.AppointmentStatus == AppointmentStatus.Approved
                    && a.Date.HasValue
                    && a.Date.Value.Date == today, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current appointment for patient: {PatientId}", patientId);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Appointment>> GetFreeSlotsByDoctorAsync(int doctorId, int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Return available future slots (appointments not yet booked by this patient)
            return await _context.Appointments
                .AsNoTracking()
                .Where(a => a.DoctorId == doctorId
                    && a.PatientId != patientId
                    && a.AppointmentStatus == AppointmentStatus.Pending
                    && a.Date.HasValue
                    && a.Date.Value > DateTime.Now)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting free slots for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }

    /// <inheritdoc/>
    public async Task<Appointment?> GetPendingFeedbackByPatientAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.PatientId == patientId
                    && a.FeedbackStatus == 2
                    && a.AppointmentStatus == AppointmentStatus.Completed, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending feedback for patient: {PatientId}", patientId);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<Appointment?> GetPatientNotificationAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.PatientId == patientId
                    && a.PatientNotification == 2
                    && a.AppointmentStatus == AppointmentStatus.Approved, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification for patient: {PatientId}", patientId);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<Appointment> AddAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync(cancellationToken);
        return appointment;
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments.FindAsync(new object[] { id }, cancellationToken);
        if (appointment != null)
        {
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments.AnyAsync(a => a.AppointId == id, cancellationToken);
    }
}
