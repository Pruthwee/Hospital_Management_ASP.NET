using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>Add staff page model.</summary>
public class AddStaffModel : PageModel
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AddStaffModel> _logger;

    public AddStaffModel(IAdminService adminService, ILogger<AddStaffModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    [BindProperty] [Required] public string Name { get; set; } = string.Empty;
    [BindProperty] public string BirthDate { get; set; } = string.Empty;
    [BindProperty] public string Phone { get; set; } = string.Empty;
    [BindProperty] public char Gender { get; set; } = 'M';
    [BindProperty] public string Address { get; set; } = string.Empty;
    [BindProperty] public int Salary { get; set; }
    [BindProperty] public string Qualification { get; set; } = string.Empty;
    [BindProperty] [Required] public string Designation { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;
    public string SuccessMessage { get; set; } = string.Empty;

    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
            return RedirectToPage("/Account/Login");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            var dto = new AddStaffDto
            {
                Name = Name, BirthDate = BirthDate, Phone = Phone,
                Gender = Gender, Address = Address, Salary = Salary,
                Qualification = Qualification, Designation = Designation
            };

            var success = await _adminService.AddStaffAsync(dto);
            if (success)
            {
                SuccessMessage = "Staff member added successfully!";
                _logger.LogInformation("New staff member added: {Name}", Name);
            }
            else
            {
                ErrorMessage = "Failed to add staff member.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding staff member: {Name}", Name);
            ErrorMessage = "An error occurred. Please try again.";
        }
        return Page();
    }
}
