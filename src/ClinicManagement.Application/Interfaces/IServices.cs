using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Application.Interfaces
{
    public interface IPatientService
    {
        Task<int> RegisterPatientAsync(Patient patient);
        Task<Patient> GetPatientByIdAsync(int id);
        Task<IEnumerable<Doctor>> GetDoctorsByDepartmentAsync(string deptName);
        Task<IEnumerable<int>> GetFreeSlotsAsync(int doctorId, int patientId);
        Task<int> RequestAppointmentAsync(int doctorId, int patientId, int slot);
        Task<Appointment> GetCurrentAppointmentAsync(int patientId);
        Task<IEnumerable<Appointment>> GetTreatmentHistoryAsync(int patientId);
    }

    public interface IDoctorService
    {
        Task<Doctor> GetDoctorByIdAsync(int id);
        Task<IEnumerable<Appointment>> GetPendingAppointmentsAsync(int doctorId);
        Task<bool> ApproveAppointmentAsync(int appointmentId);
        Task<bool> DeleteAppointmentAsync(int appointmentId);
        Task<IEnumerable<Appointment>> GetTodaysAppointmentsAsync(int doctorId);
        Task<bool> UpdatePrescriptionAsync(int doctorId, int appointmentId, string disease, string progress, string prescription);
    }

    public interface IAdminService
    {
        Task<int> AddDoctorAsync(Doctor doctor);
        Task<int> AddStaffAsync(Staff staff);
        Task<bool> DeleteDoctorAsync(int id);
        Task<bool> DeleteStaffAsync(int id);
        Task<IEnumerable<Doctor>> GetDoctorsAsync(string searchQuery);
        Task<IEnumerable<Patient>> GetPatientsAsync(string searchQuery);
        Task<IEnumerable<Staff>> GetStaffAsync(string searchQuery);
    }
}
