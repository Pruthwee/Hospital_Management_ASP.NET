using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>Today's patient history page model.</summary>
public class PatientHistoryModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<PatientHistoryModel> _logger;

    public PatientHistoryModel(IDoctorService doctorService, ILogger<PatientHistoryModel> logger)
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
            Appointments = await _doctorService.GetTodaysAppointmentsAsync(userId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading today's appointments for doctor: {UserId}", userId);
        }
        return Page();
    }
}
