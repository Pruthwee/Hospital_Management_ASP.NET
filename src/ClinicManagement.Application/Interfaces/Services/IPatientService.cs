using ClinicManagement.Application.DTOs;

namespace ClinicManagement.Application.Interfaces.Services;

/// <summary>Service interface for patient operations.</summary>
public interface IPatientService
{
    Task<PatientDto?> GetPatientByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PatientDto>> GetAllPatientsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<PatientDto>> SearchPatientsAsync(string query, CancellationToken cancellationToken = default);
    Task<BillHistoryResultDto> GetBillHistoryAsync(int patientId, CancellationToken cancellationToken = default);
    Task<TreatmentHistoryResultDto> GetTreatmentHistoryAsync(int patientId, CancellationToken cancellationToken = default);
    Task<CurrentAppointmentDto?> GetCurrentAppointmentAsync(int patientId, CancellationToken cancellationToken = default);
    Task<NotificationDto?> GetPatientNotificationAsync(int patientId, CancellationToken cancellationToken = default);
    Task<PendingFeedbackDto?> GetPendingFeedbackAsync(int patientId, CancellationToken cancellationToken = default);
    Task<bool> SubmitFeedbackAsync(int appointmentId, CancellationToken cancellationToken = default);
}
