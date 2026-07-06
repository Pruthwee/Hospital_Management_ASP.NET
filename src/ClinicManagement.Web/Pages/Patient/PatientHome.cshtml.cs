using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>Patient home page model.</summary>
public class PatientHomeModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<PatientHomeModel> _logger;

    public PatientHomeModel(IPatientService patientService, ILogger<PatientHomeModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public PatientDto? PatientInfo { get; set; }
    public CurrentAppointmentDto? CurrentAppointment { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");

        try
        {
            PatientInfo = await _patientService.GetPatientByIdAsync(userId.Value);
            CurrentAppointment = await _patientService.GetCurrentAppointmentAsync(userId.Value);

            if (PatientInfo != null)
                HttpContext.Session.SetString("UserName", PatientInfo.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading patient home for ID: {UserId}", userId);
        }
        return Page();
    }
}
