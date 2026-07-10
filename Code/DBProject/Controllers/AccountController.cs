using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace DBProject.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult SignUp() => View();

        [HttpPost]
        public IActionResult SignUp(AccountModel model)
        {
            if (ModelState.IsValid)
            {
                // Logic for sign up
                // Fixed cr-dotnet-0045: Using distributed session instead of InProc
                HttpContext.Session.SetString("UserRole", "Patient");
                return RedirectToAction("Index", "Patient");
            }
            return View(model);
        }
    }

    public class AccountModel { public string Username { get; set; } = ""; public string Password { get; set; } = ""; }
}