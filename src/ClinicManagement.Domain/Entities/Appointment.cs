using ClinicManagement.Domain.Enums;

namespace ClinicManagement.Domain.Entities;

/// <summary>Represents an appointment between a patient and a doctor.</summary>
public class Appointment
{
    public int AppointId { get; set; }
    public int? DoctorId { get; set; }
    public int? PatientId { get; set; }
    public DateTime? Date { get; set; }
    public AppointmentStatus AppointmentStatus { get; set; } = AppointmentStatus.Pending;

    public double? BillAmount { get; set; }
    public string? BillStatus { get; set; }

    public int DoctorNotification { get; set; } = 2; // 1=Seen, 2=Unseen
    public int PatientNotification { get; set; } = 2; // 1=Seen, 2=Unseen
    public int FeedbackStatus { get; set; } = 2; // 1=Given, 2=Pending

    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }

    // Navigation properties
    public Doctor? Doctor { get; set; }
    public Patient? Patient { get; set; }
}
