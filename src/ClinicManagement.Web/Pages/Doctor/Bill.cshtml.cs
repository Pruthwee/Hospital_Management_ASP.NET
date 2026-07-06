using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>Bill generation page model.</summary>
public class BillModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<BillModel> _logger;

    public BillModel(IDoctorService doctorService, ILogger<BillModel> logger)
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
            Appointments = await _doctorService.GenerateBillAsync(userId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bill for doctor: {UserId}", userId);
        }
        return Page();
    }

    public async Task<IActionResult> OnPostMarkPaidAsync(int appointmentId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");

        try
        {
            await _doctorService.MarkBillPaidAsync(userId.Value, appointmentId);
            Message = "Bill marked as paid.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking bill paid: {AppointmentId}", appointmentId);
        }

        Appointments = await _doctorService.GenerateBillAsync(userId.Value);
        return Page();
    }

    public async Task<IActionResult> OnPostMarkUnpaidAsync(int appointmentId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");

        try
        {
            await _doctorService.MarkBillUnpaidAsync(userId.Value, appointmentId);
            Message = "Bill marked as unpaid.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking bill unpaid: {AppointmentId}", appointmentId);
        }

        Appointments = await _doctorService.GenerateBillAsync(userId.Value);
        return Page();
    }
}
