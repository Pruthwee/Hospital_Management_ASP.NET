using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>Pending appointments page model.</summary>
public class PendingAppointmentModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<PendingAppointmentModel> _logger;

    public PendingAppointmentModel(IDoctorService doctorService, ILogger<PendingAppointmentModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public IEnumerable<AppointmentDto> Appointments { get; set; } = Enumerable.Empty<AppointmentDto>();
    public string Message { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");

        try
        {
            Appointments = await _doctorService.GetPendingAppointmentsAsync(userId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading pending appointments for doctor: {UserId}", userId);
        }
        return Page();
    }

    public async Task<IActionResult> OnPostApproveAsync(int appointmentId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");

        try
        {
            await _doctorService.ApproveAppointmentAsync(appointmentId);
            Message = "Appointment approved successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving appointment: {AppointmentId}", appointmentId);
            Message = "Error approving appointment.";
        }

        Appointments = await _doctorService.GetPendingAppointmentsAsync(userId.Value);
        return Page();
    }

    public async Task<IActionResult> OnPostRejectAsync(int appointmentId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");

        try
        {
            await _doctorService.DeleteAppointmentAsync(appointmentId);
            Message = "Appointment rejected.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting appointment: {AppointmentId}", appointmentId);
            Message = "Error rejecting appointment.";
        }

        Appointments = await _doctorService.GetPendingAppointmentsAsync(userId.Value);
        return Page();
    }
}
