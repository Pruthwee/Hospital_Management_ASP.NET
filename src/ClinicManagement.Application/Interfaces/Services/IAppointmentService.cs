using ClinicManagement.Application.DTOs;

namespace ClinicManagement.Application.Interfaces.Services;

/// <summary>Service interface for appointment operations.</summary>
public interface IAppointmentService
{
    Task<IEnumerable<DepartmentDto>> GetDepartmentsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentDto>> GetFreeSlotsAsync(int doctorId, int patientId, CancellationToken cancellationToken = default);
    Task<(bool Success, string Message)> BookAppointmentAsync(int doctorId, int patientId, int freeSlotId, CancellationToken cancellationToken = default);
}
