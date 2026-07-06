using AutoMapper;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>Service implementation for patient operations.</summary>
public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PatientService> _logger;

    public PatientService(
        IPatientRepository patientRepository,
        IAppointmentRepository appointmentRepository,
        IMapper mapper,
        ILogger<PatientService> logger)
    {
        _patientRepository = patientRepository;
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<PatientDto?> GetPatientByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting patient by ID: {Id}", id);
            var patient = await _patientRepository.GetByIdAsync(id, cancellationToken);
            return patient == null ? null : _mapper.Map<PatientDto>(patient);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting patient by ID: {Id}", id);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var patients = await _patientRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all patients");
            return Enumerable.Empty<PatientDto>();
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PatientDto>> SearchPatientsAsync(string query, CancellationToken cancellationToken = default)
    {
        try
        {
            var patients = await _patientRepository.SearchAsync(query, cancellationToken);
            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching patients with query: {Query}", query);
            return Enumerable.Empty<PatientDto>();
        }
    }

    /// <inheritdoc/>
    public async Task<BillHistoryResultDto> GetBillHistoryAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting bill history for patient: {PatientId}", patientId);
            var appointments = await _appointmentRepository.GetByPatientIdAsync(patientId, cancellationToken);
            var bills = appointments.Where(a => a.BillStatus != null).ToList();
            return new BillHistoryResultDto
            {
                Count = bills.Count,
                Bills = _mapper.Map<IEnumerable<AppointmentDto>>(bills)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bill history for patient: {PatientId}", patientId);
            return new BillHistoryResultDto { Count = 0, Bills = Enumerable.Empty<AppointmentDto>() };
        }
    }

    /// <inheritdoc/>
    public async Task<TreatmentHistoryResultDto> GetTreatmentHistoryAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting treatment history for patient: {PatientId}", patientId);
            var appointments = await _appointmentRepository.GetByPatientIdAsync(patientId, cancellationToken);
            var treatments = appointments.Where(a => a.Disease != null).ToList();
            return new TreatmentHistoryResultDto
            {
                Count = treatments.Count,
                Treatments = _mapper.Map<IEnumerable<AppointmentDto>>(treatments)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting treatment history for patient: {PatientId}", patientId);
            return new TreatmentHistoryResultDto { Count = 0, Treatments = Enumerable.Empty<AppointmentDto>() };
        }
    }

    /// <inheritdoc/>
    public async Task<CurrentAppointmentDto?> GetCurrentAppointmentAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetCurrentByPatientIdAsync(patientId, cancellationToken);
            if (appointment == null) return null;
            return new CurrentAppointmentDto
            {
                DoctorName = appointment.Doctor?.Name ?? string.Empty,
                Timings = appointment.Date.HasValue ? appointment.Date.Value.ToString("yyyy-MM-dd HH:mm") : string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current appointment for patient: {PatientId}", patientId);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<NotificationDto?> GetPatientNotificationAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetPatientNotificationAsync(patientId, cancellationToken);
            if (appointment == null) return null;
            return new NotificationDto
            {
                DoctorName = appointment.Doctor?.Name ?? string.Empty,
                Timings = appointment.Date.HasValue ? appointment.Date.Value.ToString("yyyy-MM-dd HH:mm") : string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification for patient: {PatientId}", patientId);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<PendingFeedbackDto?> GetPendingFeedbackAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetPendingFeedbackByPatientAsync(patientId, cancellationToken);
            if (appointment == null) return null;
            return new PendingFeedbackDto
            {
                AppointmentId = appointment.AppointId,
                DoctorName = appointment.Doctor?.Name ?? string.Empty,
                Timings = appointment.Date.HasValue ? appointment.Date.Value.ToString("yyyy-MM-dd HH:mm") : string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending feedback for patient: {PatientId}", patientId);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> SubmitFeedbackAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Submitting feedback for appointment: {AppointmentId}", appointmentId);
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null) return false;
            appointment.FeedbackStatus = 1; // Given
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting feedback for appointment: {AppointmentId}", appointmentId);
            return false;
        }
    }
}
