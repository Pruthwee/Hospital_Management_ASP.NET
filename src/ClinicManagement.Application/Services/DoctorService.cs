using AutoMapper;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>Service implementation for doctor operations.</summary>
public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ILoginRepository _loginRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<DoctorService> _logger;

    public DoctorService(
        IDoctorRepository doctorRepository,
        IAppointmentRepository appointmentRepository,
        ILoginRepository loginRepository,
        IMapper mapper,
        ILogger<DoctorService> logger)
    {
        _doctorRepository = doctorRepository;
        _appointmentRepository = appointmentRepository;
        _loginRepository = loginRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<DoctorDto?> GetDoctorByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var doctor = await _doctorRepository.GetByIdAsync(id, cancellationToken);
            return doctor == null ? null : _mapper.Map<DoctorDto>(doctor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctor by ID: {Id}", id);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<DoctorDto>> GetAllActiveDoctorsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var doctors = await _doctorRepository.GetAllActiveAsync(cancellationToken);
            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all active doctors");
            return Enumerable.Empty<DoctorDto>();
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<DoctorDto>> SearchDoctorsAsync(string query, CancellationToken cancellationToken = default)
    {
        try
        {
            var doctors = await _doctorRepository.SearchAsync(query, cancellationToken);
            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching doctors with query: {Query}", query);
            return Enumerable.Empty<DoctorDto>();
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<DoctorDto>> GetDoctorsByDepartmentAsync(string departmentName, CancellationToken cancellationToken = default)
    {
        try
        {
            var doctors = await _doctorRepository.GetByDepartmentAsync(departmentName, cancellationToken);
            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctors by department: {Department}", departmentName);
            return Enumerable.Empty<DoctorDto>();
        }
    }

    /// <inheritdoc/>
    public async Task<bool> AddDoctorAsync(AddDoctorDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Adding new doctor with email: {Email}", dto.Email);
            if (await _loginRepository.EmailExistsAsync(dto.Email, cancellationToken))
                return false;

            var login = new LoginTable
            {
                Email = dto.Email,
                Password = dto.Password,
                Type = UserType.Doctor
            };
            var createdLogin = await _loginRepository.AddAsync(login, cancellationToken);

            var doctor = new Doctor
            {
                DoctorId = createdLogin.LoginId,
                Name = dto.Name,
                Phone = dto.Phone,
                Address = dto.Address,
                BirthDate = DateTime.Parse(dto.BirthDate),
                DeptNo = dto.DeptNo,
                Gender = dto.Gender,
                ChargesPerVisit = dto.ChargesPerVisit,
                MonthlySalary = dto.Salary,
                Qualification = dto.Qualification,
                Specialization = dto.Specialization,
                WorkExperience = dto.Experience,
                Status = DoctorStatus.Present
            };
            await _doctorRepository.AddAsync(doctor, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding doctor with email: {Email}", dto.Email);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteDoctorAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting (deactivating) doctor: {Id}", id);
            var doctor = await _doctorRepository.GetByIdAsync(id, cancellationToken);
            if (doctor == null) return false;
            doctor.Status = DoctorStatus.Left;
            await _doctorRepository.UpdateAsync(doctor, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting doctor: {Id}", id);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _doctorRepository.EmailExistsAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking email existence: {Email}", email);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<AppointmentDto>> GetPendingAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointments = await _appointmentRepository.GetPendingByDoctorIdAsync(doctorId, cancellationToken);
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending appointments for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<AppointmentDto>> GetTodaysAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointments = await _appointmentRepository.GetTodaysByDoctorIdAsync(doctorId, cancellationToken);
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting today's appointments for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<AppointmentDto>> GetPatientHistoryAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointments = await _appointmentRepository.GetByDoctorIdAsync(doctorId, cancellationToken);
            var history = appointments.Where(a => a.AppointmentStatus == AppointmentStatus.Completed).ToList();
            return _mapper.Map<IEnumerable<AppointmentDto>>(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting patient history for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    /// <inheritdoc/>
    public async Task<bool> ApproveAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Approving appointment: {AppointmentId}", appointmentId);
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null) return false;
            appointment.AppointmentStatus = AppointmentStatus.Approved;
            appointment.DoctorNotification = 1;
            appointment.PatientNotification = 2;
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving appointment: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting appointment: {AppointmentId}", appointmentId);
            await _appointmentRepository.DeleteAsync(appointmentId, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting appointment: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> UpdatePrescriptionAsync(int doctorId, int appointmentId, string disease, string progress, string prescription, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating prescription for appointment: {AppointmentId}", appointmentId);
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null || appointment.DoctorId != doctorId) return false;
            appointment.Disease = disease;
            appointment.Progress = progress;
            appointment.Prescription = prescription;
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating prescription for appointment: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<AppointmentDto>> GenerateBillAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointments = await _appointmentRepository.GetTodaysByDoctorIdAsync(doctorId, cancellationToken);
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating bill for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    /// <inheritdoc/>
    public async Task<bool> MarkBillPaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null || appointment.DoctorId != doctorId) return false;
            appointment.BillStatus = "Paid";
            appointment.AppointmentStatus = AppointmentStatus.Completed;
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking bill paid for appointment: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> MarkBillUnpaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null || appointment.DoctorId != doctorId) return false;
            appointment.BillStatus = "Unpaid";
            appointment.AppointmentStatus = AppointmentStatus.Completed;
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking bill unpaid for appointment: {AppointmentId}", appointmentId);
            return false;
        }
    }
}
