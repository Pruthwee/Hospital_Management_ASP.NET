using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>Doctor profile page model.</summary>
public class DoctorProfileModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<DoctorProfileModel> _logger;

    public DoctorProfileModel(IDoctorService doctorService, ILogger<DoctorProfileModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    [BindProperty(SupportsGet = true)]
    public int DoctorId { get; set; }

    public DoctorDto? Doctor { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
            return RedirectToPage("/Account/Login");

        try
        {
            Doctor = await _doctorService.GetDoctorByIdAsync(DoctorId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctor profile: {DoctorId}", DoctorId);
        }
        return Page();
    }
}
