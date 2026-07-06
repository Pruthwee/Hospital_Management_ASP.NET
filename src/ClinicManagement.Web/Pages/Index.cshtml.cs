using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages;

/// <summary>Index page model.</summary>
public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        return RedirectToPage("/Account/Login");
    }
}
