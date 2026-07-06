using AutoMapper;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>Service implementation for admin operations.</summary>
public class AdminService : IAdminService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<AdminService> _logger;

    public AdminService(
        IDoctorRepository doctorRepository,
        IPatientRepository patientRepository,
        IStaffRepository staffRepository,
        IDepartmentRepository departmentRepository,
        IAppointmentRepository appointmentRepository,
        IMapper mapper,
        ILogger<AdminService> logger)
    {
        _doctorRepository = doctorRepository;
        _patientRepository = patientRepository;
        _staffRepository = staffRepository;
        _departmentRepository = departmentRepository;
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<AdminHomeDto> GetAdminHomeDataAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting admin home data");
            var patients = await _patientRepository.GetAllAsync(cancellationToken);
            var doctors = await _doctorRepository.GetAllActiveAsync(cancellationToken);
            var departments = await _departmentRepository.GetAllAsync(cancellationToken);

            return new AdminHomeDto
            {
                TotalPatients = patients.Count(),
                TotalDoctors = doctors.Count(),
                TotalIncome = 0,
                Departments = _mapper.Map<IEnumerable<DepartmentDto>>(departments),
                RecentAppointments = Enumerable.Empty<AppointmentDto>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting admin home data");
            return new AdminHomeDto();
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync(string searchQuery = "", CancellationToken cancellationToken = default)
    {
        try
        {
            IEnumerable<Domain.Entities.Doctor> doctors;
            if (string.IsNullOrWhiteSpace(searchQuery))
                doctors = await _doctorRepository.GetAllActiveAsync(cancellationToken);
            else
                doctors = await _doctorRepository.SearchAsync(searchQuery, cancellationToken);
            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all doctors");
            return Enumerable.Empty<DoctorDto>();
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync(string searchQuery = "", CancellationToken cancellationToken = default)
    {
        try
        {
            IEnumerable<Domain.Entities.Patient> patients;
            if (string.IsNullOrWhiteSpace(searchQuery))
                patients = await _patientRepository.GetAllAsync(cancellationToken);
            else
                patients = await _patientRepository.SearchAsync(searchQuery, cancellationToken);
            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all patients");
            return Enumerable.Empty<PatientDto>();
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<StaffDto>> GetAllStaffAsync(string searchQuery = "", CancellationToken cancellationToken = default)
    {
        try
        {
            IEnumerable<OtherStaff> staff;
            if (string.IsNullOrWhiteSpace(searchQuery))
                staff = await _staffRepository.GetAllAsync(cancellationToken);
            else
                staff = await _staffRepository.SearchAsync(searchQuery, cancellationToken);
            return _mapper.Map<IEnumerable<StaffDto>>(staff);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all staff");
            return Enumerable.Empty<StaffDto>();
        }
    }

    /// <inheritdoc/>
    public async Task<PatientDto?> GetPatientDetailsAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var patient = await _patientRepository.GetByIdAsync(patientId, cancellationToken);
            return patient == null ? null : _mapper.Map<PatientDto>(patient);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting patient details: {PatientId}", patientId);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<StaffDto?> GetStaffDetailsAsync(int staffId, CancellationToken cancellationToken = default)
    {
        try
        {
            var staff = await _staffRepository.GetByIdAsync(staffId, cancellationToken);
            return staff == null ? null : _mapper.Map<StaffDto>(staff);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting staff details: {StaffId}", staffId);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> AddStaffAsync(AddStaffDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Adding new staff member: {Name}", dto.Name);
            var staff = new OtherStaff
            {
                Name = dto.Name,
                Phone = dto.Phone,
                Address = dto.Address,
                Gender = dto.Gender,
                Designation = dto.Designation,
                Salary = dto.Salary,
                HighestQualification = dto.Qualification,
                BirthDate = string.IsNullOrEmpty(dto.BirthDate) ? null : DateTime.Parse(dto.BirthDate)
            };
            await _staffRepository.AddAsync(staff, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding staff member: {Name}", dto.Name);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteDoctorAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting doctor: {DoctorId}", doctorId);
            var doctor = await _doctorRepository.GetByIdAsync(doctorId, cancellationToken);
            if (doctor == null) return false;
            doctor.Status = Domain.Enums.DoctorStatus.Left;
            await _doctorRepository.UpdateAsync(doctor, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting doctor: {DoctorId}", doctorId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteStaffAsync(int staffId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting staff: {StaffId}", staffId);
            await _staffRepository.DeleteAsync(staffId, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff: {StaffId}", staffId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var departments = await _departmentRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<DepartmentDto>>(departments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all departments");
            return Enumerable.Empty<DepartmentDto>();
        }
    }
}
