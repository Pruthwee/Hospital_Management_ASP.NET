using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Web.Pages.Account;

/// <summary>Patient registration page model.</summary>
public class RegisterModel : PageModel
{
    private readonly IAuthService _authService;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(IAuthService authService, ILogger<RegisterModel> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [BindProperty]
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(30)]
    public string Name { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Phone is required")]
    public string Phone { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Birth date is required")]
    public string BirthDate { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Gender is required")]
    public string Gender { get; set; } = string.Empty;

    [BindProperty]
    public string Address { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;
    public string SuccessMessage { get; set; } = string.Empty;

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            var (success, userId, message) = await _authService.RegisterPatientAsync(
                Name, BirthDate, Email, Password, Phone, Gender, Address);

            if (!success)
            {
                ErrorMessage = message;
                return Page();
            }

            _logger.LogInformation("New patient registered with ID: {UserId}", userId);
            SuccessMessage = "Registration successful! You can now login.";
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during patient registration for email: {Email}", Email);
            ErrorMessage = "An error occurred during registration. Please try again.";
            return Page();
        }
    }
}
