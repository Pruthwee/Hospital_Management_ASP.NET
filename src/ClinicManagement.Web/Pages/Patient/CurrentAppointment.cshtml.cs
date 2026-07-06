using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>Current appointment page model.</summary>
public class CurrentAppointmentModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<CurrentAppointmentModel> _logger;

    public CurrentAppointmentModel(IPatientService patientService, ILogger<CurrentAppointmentModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public CurrentAppointmentDto? CurrentAppointment { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");

        try
        {
            CurrentAppointment = await _patientService.GetCurrentAppointmentAsync(userId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading current appointment for patient: {UserId}", userId);
        }
        return Page();
    }
}
