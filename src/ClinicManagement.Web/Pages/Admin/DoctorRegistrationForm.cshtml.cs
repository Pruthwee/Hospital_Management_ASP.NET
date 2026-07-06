using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>Doctor registration form page model.</summary>
public class DoctorRegistrationFormModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly IAdminService _adminService;
    private readonly ILogger<DoctorRegistrationFormModel> _logger;

    public DoctorRegistrationFormModel(
        IDoctorService doctorService,
        IAdminService adminService,
        ILogger<DoctorRegistrationFormModel> logger)
    {
        _doctorService = doctorService;
        _adminService = adminService;
        _logger = logger;
    }

    [BindProperty] [Required] public string Name { get; set; } = string.Empty;
    [BindProperty] [Required] [EmailAddress] public string Email { get; set; } = string.Empty;
    [BindProperty] [Required] public string Password { get; set; } = string.Empty;
    [BindProperty] [Required] public string BirthDate { get; set; } = string.Empty;
    [BindProperty] [Required] public int DeptNo { get; set; }
    [BindProperty] public string Phone { get; set; } = string.Empty;
    [BindProperty] public char Gender { get; set; } = 'M';
    [BindProperty] public string Address { get; set; } = string.Empty;
    [BindProperty] public int Experience { get; set; }
    [BindProperty] public int Salary { get; set; }
    [BindProperty] public int ChargesPerVisit { get; set; }
    [BindProperty] public string Specialization { get; set; } = string.Empty;
    [BindProperty] [Required] public string Qualification { get; set; } = string.Empty;

    public IEnumerable<DepartmentDto> Departments { get; set; } = Enumerable.Empty<DepartmentDto>();
    public string ErrorMessage { get; set; } = string.Empty;
    public string SuccessMessage { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
            return RedirectToPage("/Account/Login");

        Departments = await _adminService.GetAllDepartmentsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Departments = await _adminService.GetAllDepartmentsAsync();

        if (!ModelState.IsValid)
            return Page();

        try
        {
            if (await _doctorService.EmailExistsAsync(Email))
            {
                ErrorMessage = "Email already exists.";
                return Page();
            }

            var dto = new AddDoctorDto
            {
                Name = Name, Email = Email, Password = Password,
                BirthDate = BirthDate, DeptNo = DeptNo, Phone = Phone,
                Gender = Gender, Address = Address, Experience = Experience,
                Salary = Salary, ChargesPerVisit = ChargesPerVisit,
                Specialization = Specialization, Qualification = Qualification
            };

            var success = await _doctorService.AddDoctorAsync(dto);
            if (success)
            {
                SuccessMessage = "Doctor registered successfully!";
                _logger.LogInformation("New doctor registered: {Email}", Email);
            }
            else
            {
                ErrorMessage = "Failed to register doctor.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering doctor: {Email}", Email);
            ErrorMessage = "An error occurred. Please try again.";
        }
        return Page();
    }
}
