using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>History update page model.</summary>
public class HistoryUpdateModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<HistoryUpdateModel> _logger;

    public HistoryUpdateModel(IDoctorService doctorService, ILogger<HistoryUpdateModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    [BindProperty(SupportsGet = true)]
    public int AppointmentId { get; set; }

    [BindProperty] public string Disease { get; set; } = string.Empty;
    [BindProperty] public string Progress { get; set; } = string.Empty;
    [BindProperty] public string Prescription { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
            return RedirectToPage("/Account/Login");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");

        try
        {
            var success = await _doctorService.UpdatePrescriptionAsync(
                userId.Value, AppointmentId, Disease, Progress, Prescription);
            Message = success ? "History updated successfully!" : "Failed to update history.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating history for appointment: {AppointmentId}", AppointmentId);
            Message = "An error occurred.";
        }
        return Page();
    }
}
