using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Application.Services
{
    public class PatientService : IPatientService
    {
        private readonly ClinicDbContext _context;
        public PatientService(ClinicDbContext context) { _context = context; }

        public async Task<int> RegisterPatientAsync(Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return patient.PatientID;
        }

        public async Task<Patient> GetPatientByIdAsync(int id) => await _context.Patients.FindAsync(id);

        public async Task<IEnumerable<Doctor>> GetDoctorsByDepartmentAsync(string deptName)
        {
            // Simplified for demo, in real app would join with Department table
            return await _context.Doctors.Where(d => d.Specialization == deptName).ToListAsync();
        }

        public async Task<IEnumerable<int>> GetFreeSlotsAsync(int doctorId, int patientId)
        {
            // Mocking free slots logic
            return new List<int> { 1, 2, 3, 4, 5 };
        }

        public async Task<int> RequestAppointmentAsync(int doctorId, int patientId, int slot)
        {
            var appointment = new Appointment { DoctorID = doctorId, PatientID = patientId, FreeSlot = slot, Status = "Pending", AppointmentDate = DateTime.Now };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return appointment.AppointmentID;
        }

        public async Task<Appointment> GetCurrentAppointmentAsync(int patientId)
        {
            return await _context.Appointments.FirstOrDefaultAsync(a => a.PatientID == patientId && a.Status == "Approved");
        }

        public async Task<IEnumerable<Appointment>> GetTreatmentHistoryAsync(int patientId)
        {
            return await _context.Appointments.Where(a => a.PatientID == patientId).ToListAsync();
        }
    }

    public class DoctorService : IDoctorService
    {
        private readonly ClinicDbContext _context;
        public DoctorService(ClinicDbContext context) { _context = context; }

        public async Task<Doctor> GetDoctorByIdAsync(int id) => await _context.Doctors.FindAsync(id);

        public async Task<IEnumerable<Appointment>> GetPendingAppointmentsAsync(int doctorId)
        {
            return await _context.Appointments.Where(a => a.DoctorID == doctorId && a.Status == "Pending").ToListAsync();
        }

        public async Task<bool> ApproveAppointmentAsync(int appointmentId)
        {
            var app = await _context.Appointments.FindAsync(appointmentId);
            if (app == null) return false;
            app.Status = "Approved";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAppointmentAsync(int appointmentId)
        {
            var app = await _context.Appointments.FindAsync(appointmentId);
            if (app == null) return false;
            _context.Appointments.Remove(app);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Appointment>> GetTodaysAppointmentsAsync(int doctorId)
        {
            return await _context.Appointments.Where(a => a.DoctorID == doctorId && a.AppointmentDate.Date == DateTime.Today).ToListAsync();
        }

        public async Task<bool> UpdatePrescriptionAsync(int doctorId, int appointmentId, string disease, string progress, string prescription)
        {
            var app = await _context.Appointments.FindAsync(appointmentId);
            if (app == null || app.DoctorID != doctorId) return false;
            app.Disease = disease;
            app.Progress = progress;
            app.Prescription = prescription;
            app.Status = "Completed";
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class AdminService : IAdminService
    {
        private readonly ClinicDbContext _context;
        public AdminService(ClinicDbContext context) { _context = context; }

        public async Task<int> AddDoctorAsync(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
            return doctor.DoctorID;
        }

        public async Task<int> AddStaffAsync(Staff staff)
        {
            _context.Staffs.Add(staff);
            await _context.SaveChangesAsync();
            return staff.StaffID;
        }

        public async Task<bool> DeleteDoctorAsync(int id)
        {
            var doc = await _context.Doctors.FindAsync(id);
            if (doc == null) return false;
            doc.Status = 0; // Soft delete
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteStaffAsync(int id)
        {
            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null) return false;
            _context.Staffs.Remove(staff);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Doctor>> GetDoctorsAsync(string searchQuery)
        {
            return await _context.Doctors.Where(d => string.IsNullOrEmpty(searchQuery) || d.Name.Contains(searchQuery)).ToListAsync();
        }

        public async Task<IEnumerable<Patient>> GetPatientsAsync(string searchQuery)
        {
            return await _context.Patients.Where(p => string.IsNullOrEmpty(searchQuery) || p.Name.Contains(searchQuery)).ToListAsync();
        }

        public async Task<IEnumerable<Staff>> GetStaffAsync(string searchQuery)
        {
            return await _context.Staffs.Where(s => string.IsNullOrEmpty(searchQuery) || s.Name.Contains(searchQuery)).ToListAsync();
        }
    }
}
