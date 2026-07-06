using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>Admin home page model.</summary>
public class AdminHomeModel : PageModel
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AdminHomeModel> _logger;

    public AdminHomeModel(IAdminService adminService, ILogger<AdminHomeModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    public AdminHomeDto HomeData { get; set; } = new AdminHomeDto();

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
            return RedirectToPage("/Account/Login");

        try
        {
            HomeData = await _adminService.GetAdminHomeDataAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin home data");
        }
        return Page();
    }
}
