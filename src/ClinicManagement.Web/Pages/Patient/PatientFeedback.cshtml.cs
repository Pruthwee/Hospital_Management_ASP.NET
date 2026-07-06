using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>Patient feedback page model.</summary>
public class PatientFeedbackModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<PatientFeedbackModel> _logger;

    public PatientFeedbackModel(IPatientService patientService, ILogger<PatientFeedbackModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public PendingFeedbackDto? PendingFeedback { get; set; }
    public string Message { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");

        try
        {
            PendingFeedback = await _patientService.GetPendingFeedbackAsync(userId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading feedback for patient: {UserId}", userId);
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int appointmentId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");

        try
        {
            var success = await _patientService.SubmitFeedbackAsync(appointmentId);
            Message = success ? "Feedback submitted successfully!" : "Failed to submit feedback.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting feedback for appointment: {AppointmentId}", appointmentId);
            Message = "An error occurred.";
        }

        PendingFeedback = await _patientService.GetPendingFeedbackAsync(userId.Value);
        return Page();
    }
}
