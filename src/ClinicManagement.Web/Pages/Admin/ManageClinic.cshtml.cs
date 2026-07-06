using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>Manage clinic page model.</summary>
public class ManageClinicModel : PageModel
{
    private readonly IAdminService _adminService;
    private readonly ILogger<ManageClinicModel> _logger;

    public ManageClinicModel(IAdminService adminService, ILogger<ManageClinicModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    public IEnumerable<DoctorDto> Doctors { get; set; } = Enumerable.Empty<DoctorDto>();
    public IEnumerable<PatientDto> Patients { get; set; } = Enumerable.Empty<PatientDto>();
    public IEnumerable<StaffDto> Staff { get; set; } = Enumerable.Empty<StaffDto>();

    [BindProperty(SupportsGet = true)]
    public string DoctorSearch { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string PatientSearch { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string StaffSearch { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
            return RedirectToPage("/Account/Login");

        try
        {
            Doctors = await _adminService.GetAllDoctorsAsync(DoctorSearch);
            Patients = await _adminService.GetAllPatientsAsync(PatientSearch);
            Staff = await _adminService.GetAllStaffAsync(StaffSearch);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading manage clinic data");
        }
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteDoctorAsync(int doctorId)
    {
        try
        {
            await _adminService.DeleteDoctorAsync(doctorId);
            _logger.LogInformation("Doctor {DoctorId} deleted by admin", doctorId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting doctor: {DoctorId}", doctorId);
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteStaffAsync(int staffId)
    {
        try
        {
            await _adminService.DeleteStaffAsync(staffId);
            _logger.LogInformation("Staff {StaffId} deleted by admin", staffId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff: {StaffId}", staffId);
        }
        return RedirectToPage();
    }
}
