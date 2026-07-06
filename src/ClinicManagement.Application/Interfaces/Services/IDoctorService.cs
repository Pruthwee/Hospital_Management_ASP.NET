using ClinicManagement.Application.DTOs;

namespace ClinicManagement.Application.Interfaces.Services;

/// <summary>Service interface for doctor operations.</summary>
public interface IDoctorService
{
    Task<DoctorDto?> GetDoctorByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<DoctorDto>> GetAllActiveDoctorsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<DoctorDto>> SearchDoctorsAsync(string query, CancellationToken cancellationToken = default);
    Task<IEnumerable<DoctorDto>> GetDoctorsByDepartmentAsync(string departmentName, CancellationToken cancellationToken = default);
    Task<bool> AddDoctorAsync(AddDoctorDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteDoctorAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentDto>> GetPendingAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentDto>> GetTodaysAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentDto>> GetPatientHistoryAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<bool> ApproveAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default);
    Task<bool> UpdatePrescriptionAsync(int doctorId, int appointmentId, string disease, string progress, string prescription, CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentDto>> GenerateBillAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<bool> MarkBillPaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default);
    Task<bool> MarkBillUnpaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default);
}
