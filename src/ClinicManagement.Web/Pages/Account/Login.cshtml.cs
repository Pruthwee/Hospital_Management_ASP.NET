using ClinicManagement.Domain.Enums;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Web.Pages.Account;

/// <summary>Login page model.</summary>
public class LoginModel : PageModel
{
    private readonly IAuthService _authService;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(IAuthService authService, ILogger<LoginModel> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [BindProperty]
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;

    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetInt32("UserId") != null)
        {
            return RedirectBasedOnRole();
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            var (success, userType, userId) = await _authService.ValidateLoginAsync(Email, Password);

            if (!success)
            {
                ErrorMessage = "Invalid email or password.";
                return Page();
            }

            HttpContext.Session.SetInt32("UserId", userId);
            HttpContext.Session.SetInt32("UserType", (int)userType);
            HttpContext.Session.SetString("UserEmail", Email);

            _logger.LogInformation("User {Email} logged in successfully as {UserType}", Email, userType);

            return userType switch
            {
                UserType.Admin => RedirectToPage("/Admin/AdminHome"),
                UserType.Doctor => RedirectToPage("/Doctor/DoctorHome"),
                UserType.Patient => RedirectToPage("/Patient/PatientHome"),
                _ => RedirectToPage("/Account/Login")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", Email);
            ErrorMessage = "An error occurred during login. Please try again.";
            return Page();
        }
    }

    private IActionResult RedirectBasedOnRole()
    {
        var userType = (UserType?)HttpContext.Session.GetInt32("UserType");
        return userType switch
        {
            UserType.Admin => RedirectToPage("/Admin/AdminHome"),
            UserType.Doctor => RedirectToPage("/Doctor/DoctorHome"),
            UserType.Patient => RedirectToPage("/Patient/PatientHome"),
            _ => Page()
        };
    }
}
