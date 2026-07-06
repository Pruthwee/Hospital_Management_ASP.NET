using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>Patient notifications page model.</summary>
public class PatientNotificationsModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<PatientNotificationsModel> _logger;

    public PatientNotificationsModel(IPatientService patientService, ILogger<PatientNotificationsModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public NotificationDto? Notification { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");

        try
        {
            Notification = await _patientService.GetPatientNotificationAsync(userId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading notifications for patient: {UserId}", userId);
        }
        return Page();
    }
}
