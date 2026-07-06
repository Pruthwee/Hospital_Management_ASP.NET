using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>Previous history page model.</summary>
public class PreviousHistoryModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<PreviousHistoryModel> _logger;

    public PreviousHistoryModel(IDoctorService doctorService, ILogger<PreviousHistoryModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public IEnumerable<AppointmentDto> Appointments { get; set; } = Enumerable.Empty<AppointmentDto>();

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");

        try
        {
            Appointments = await _doctorService.GetPatientHistoryAsync(userId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading previous history for doctor: {UserId}", userId);
        }
        return Page();
    }
}
