using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>Doctor home page model.</summary>
public class DoctorHomeModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<DoctorHomeModel> _logger;

    public DoctorHomeModel(IDoctorService doctorService, ILogger<DoctorHomeModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public DoctorDto? DoctorInfo { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToPage("/Account/Login");

        try
        {
            DoctorInfo = await _doctorService.GetDoctorByIdAsync(userId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctor home for ID: {UserId}", userId);
        }
        return Page();
    }
}
