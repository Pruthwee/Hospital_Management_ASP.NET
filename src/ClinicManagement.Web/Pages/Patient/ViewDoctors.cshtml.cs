using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>View doctors page model.</summary>
public class ViewDoctorsModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly IAdminService _adminService;
    private readonly ILogger<ViewDoctorsModel> _logger;

    public ViewDoctorsModel(IDoctorService doctorService, IAdminService adminService, ILogger<ViewDoctorsModel> logger)
    {
        _doctorService = doctorService;
        _adminService = adminService;
        _logger = logger;
    }

    public IEnumerable<DoctorDto> Doctors { get; set; } = Enumerable.Empty<DoctorDto>();
    public IEnumerable<DepartmentDto> Departments { get; set; } = Enumerable.Empty<DepartmentDto>();

    [BindProperty(SupportsGet = true)]
    public string SelectedDepartment { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
            return RedirectToPage("/Account/Login");

        try
        {
            Departments = await _adminService.GetAllDepartmentsAsync();
            if (!string.IsNullOrEmpty(SelectedDepartment))
                Doctors = await _doctorService.GetDoctorsByDepartmentAsync(SelectedDepartment);
            else
                Doctors = await _doctorService.GetAllActiveDoctorsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctors");
        }
        return Page();
    }
}
