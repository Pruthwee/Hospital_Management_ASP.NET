using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>Treatment history page model.</summary>
public class TreatmentHistoryModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<TreatmentHistoryModel> _logger;

    public TreatmentHistoryModel(IPatientService patientService, ILogger<TreatmentHistoryModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public IEnumerable<AppointmentDto> Treatments { get; set; } = Enumerable.Empty<AppointmentDto>();

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");

        try
        {
            var result = await _patientService.GetTreatmentHistoryAsync(userId.Value);
            Treatments = result.Treatments;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading treatment history for patient: {UserId}", userId);
        }
        return Page();
    }
}
