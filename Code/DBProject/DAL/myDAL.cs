using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<PatientHistory> PatientHistories { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentRequest> AppointmentRequests { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<User> Users { get; set; }
    }

    public class Staff { public int StaffId { get; set; } public string Name { get; set; } = ""; public string Role { get; set; } = ""; public string Email { get; set; } = ""; }
    public class Doctor { public int DoctorId { get; set; } public string Name { get; set; } = ""; public string Specialization { get; set; } = ""; public string Email { get; set; } = ""; }
    public class Clinic { public int ClinicId { get; set; } public string Name { get; set; } = ""; public string Location { get; set; } = ""; }
    public class Bill { public int BillId { get; set; } public int PatientId { get; set; } public decimal Amount { get; set; } public DateTime Date { get; set; } }
    public class PatientHistory { public int HistoryId { get; set; } public int PatientId { get; set; } public string Diagnosis { get; set; } = ""; public string Treatment { get; set; } = ""; }
    public class Appointment { public int AppointmentId { get; set; } public int PatientId { get; set; } public DateTime Date { get; set; } public string Status { get; set; } = ""; }
    public class AppointmentRequest { public int RequestId { get; set; } public int PatientId { get; set; } public int DoctorId { get; set; } public DateTime Date { get; set; } }
    public class Notification { public int NotificationId { get; set; } public int PatientId { get; set; } public string Message { get; set; } = ""; public DateTime Date { get; set; } }
    public class Feedback { public int FeedbackId { get; set; } public int PatientId { get; set; } public string Comments { get; set; } = ""; public int Rating { get; set; } }
    public class User { public int UserId { get; set; } public string Username { get; set; } = ""; public string Password { get; set; } = ""; public string Role { get; set; } = ""; }
}
