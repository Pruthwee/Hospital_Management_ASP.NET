using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>Take appointment page model.</summary>
public class TakeAppointmentModel : PageModel
{
    private readonly IAppointmentService _appointmentService;
    private readonly IDoctorService _doctorService;
    private readonly ILogger<TakeAppointmentModel> _logger;

    public TakeAppointmentModel(
        IAppointmentService appointmentService,
        IDoctorService doctorService,
        ILogger<TakeAppointmentModel> logger)
    {
        _appointmentService = appointmentService;
        _doctorService = doctorService;
        _logger = logger;
    }

    public IEnumerable<DepartmentDto> Departments { get; set; } = Enumerable.Empty<DepartmentDto>();
    public IEnumerable<DoctorDto> Doctors { get; set; } = Enumerable.Empty<DoctorDto>();
    public IEnumerable<AppointmentDto> FreeSlots { get; set; } = Enumerable.Empty<AppointmentDto>();

    [BindProperty(SupportsGet = true)]
    public string SelectedDepartment { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public int SelectedDoctorId { get; set; }

    public string Message { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");

        try
        {
            Departments = await _appointmentService.GetDepartmentsAsync();

            if (!string.IsNullOrEmpty(SelectedDepartment))
                Doctors = await _doctorService.GetDoctorsByDepartmentAsync(SelectedDepartment);

            if (SelectedDoctorId > 0)
                FreeSlots = await _appointmentService.GetFreeSlotsAsync(SelectedDoctorId, userId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading take appointment page");
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int doctorId, int slotId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");

        try
        {
            var (success, message) = await _appointmentService.BookAppointmentAsync(doctorId, userId.Value, slotId);
            if (success)
                return RedirectToPage("/Patient/AppointmentRequestSent");

            Message = message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking appointment");
            Message = "An error occurred while booking the appointment.";
        }

        Departments = await _appointmentService.GetDepartmentsAsync();
        return Page();
    }
}
