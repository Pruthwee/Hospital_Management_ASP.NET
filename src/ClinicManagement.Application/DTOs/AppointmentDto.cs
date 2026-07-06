namespace ClinicManagement.Application.DTOs;

/// <summary>DTO for appointment data.</summary>
public class AppointmentDto
{
    public int AppointId { get; set; }
    public int? DoctorId { get; set; }
    public int? PatientId { get; set; }
    public string? DoctorName { get; set; }
    public string? PatientName { get; set; }
    public DateTime? Date { get; set; }
    public string? Timings { get; set; }
    public string AppointmentStatus { get; set; } = string.Empty;
    public double? BillAmount { get; set; }
    public string? BillStatus { get; set; }
    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }
    public int FeedbackStatus { get; set; }
}

/// <summary>DTO for current appointment display.</summary>
public class CurrentAppointmentDto
{
    public string DoctorName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
}

/// <summary>DTO for notification data.</summary>
public class NotificationDto
{
    public string DoctorName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
}

/// <summary>DTO for pending feedback data.</summary>
public class PendingFeedbackDto
{
    public int AppointmentId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
}

/// <summary>DTO for bill history result.</summary>
public class BillHistoryResultDto
{
    public int Count { get; set; }
    public IEnumerable<AppointmentDto> Bills { get; set; } = new List<AppointmentDto>();
}

/// <summary>DTO for treatment history result.</summary>
public class TreatmentHistoryResultDto
{
    public int Count { get; set; }
    public IEnumerable<AppointmentDto> Treatments { get; set; } = new List<AppointmentDto>();
}
