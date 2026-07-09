using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Domain.Entities
{
    public class Appointment
    {
        [Key]
        public int AppointmentID { get; set; }
        public int DoctorID { get; set; }
        public int PatientID { get; set; }
        public int FreeSlot { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } // Pending, Approved, Completed, Cancelled
        public string Disease { get; set; }
        public string Progress { get; set; }
        public string Prescription { get; set; }
    }
}
