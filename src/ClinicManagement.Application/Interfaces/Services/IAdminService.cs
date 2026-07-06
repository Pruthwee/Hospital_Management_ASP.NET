using ClinicManagement.Application.DTOs;

namespace ClinicManagement.Application.Interfaces.Services;

/// <summary>Service interface for admin operations.</summary>
public interface IAdminService
{
    Task<AdminHomeDto> GetAdminHomeDataAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync(string searchQuery = "", CancellationToken cancellationToken = default);
    Task<IEnumerable<PatientDto>> GetAllPatientsAsync(string searchQuery = "", CancellationToken cancellationToken = default);
    Task<IEnumerable<StaffDto>> GetAllStaffAsync(string searchQuery = "", CancellationToken cancellationToken = default);
    Task<PatientDto?> GetPatientDetailsAsync(int patientId, CancellationToken cancellationToken = default);
    Task<StaffDto?> GetStaffDetailsAsync(int staffId, CancellationToken cancellationToken = default);
    Task<bool> AddStaffAsync(AddStaffDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteDoctorAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<bool> DeleteStaffAsync(int staffId, CancellationToken cancellationToken = default);
    Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync(CancellationToken cancellationToken = default);
}
