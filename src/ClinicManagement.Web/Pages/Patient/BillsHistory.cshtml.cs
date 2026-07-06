using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>Bills history page model.</summary>
public class BillsHistoryModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<BillsHistoryModel> _logger;

    public BillsHistoryModel(IPatientService patientService, ILogger<BillsHistoryModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public IEnumerable<AppointmentDto> Bills { get; set; } = Enumerable.Empty<AppointmentDto>();

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");

        try
        {
            var result = await _patientService.GetBillHistoryAsync(userId.Value);
            Bills = result.Bills;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bills history for patient: {UserId}", userId);
        }
        return Page();
    }
}
