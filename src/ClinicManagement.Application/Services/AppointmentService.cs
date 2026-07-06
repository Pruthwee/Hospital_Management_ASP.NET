using AutoMapper;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>Service implementation for appointment operations.</summary>
public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        IDepartmentRepository departmentRepository,
        IMapper mapper,
        ILogger<AppointmentService> logger)
    {
        _appointmentRepository = appointmentRepository;
        _departmentRepository = departmentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<DepartmentDto>> GetDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var departments = await _departmentRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<DepartmentDto>>(departments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting departments");
            return Enumerable.Empty<DepartmentDto>();
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<AppointmentDto>> GetFreeSlotsAsync(int doctorId, int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var slots = await _appointmentRepository.GetFreeSlotsByDoctorAsync(doctorId, patientId, cancellationToken);
            return _mapper.Map<IEnumerable<AppointmentDto>>(slots);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting free slots for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    /// <inheritdoc/>
    public async Task<(bool Success, string Message)> BookAppointmentAsync(
        int doctorId, int patientId, int freeSlotId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Booking appointment for patient: {PatientId} with doctor: {DoctorId}", patientId, doctorId);
            var appointment = new Appointment
            {
                DoctorId = doctorId,
                PatientId = patientId,
                Date = DateTime.Now.AddDays(freeSlotId),
                AppointmentStatus = AppointmentStatus.Pending,
                DoctorNotification = 2,
                PatientNotification = 2,
                FeedbackStatus = 2
            };
            await _appointmentRepository.AddAsync(appointment, cancellationToken);
            return (true, "Appointment request sent successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking appointment for patient: {PatientId}", patientId);
            return (false, "Failed to book appointment.");
        }
    }
}
