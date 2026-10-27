using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using DBProject.DAL;

namespace DBProject
{
    /// <summary>
    /// Razor Page model for SignUp — replaces ASP.NET Web Forms SignUp.aspx / SignUp.aspx.cs.
    /// Eliminates ViewState, postbacks, and server affinity; enables stateless horizontal scaling on Azure Container Apps.
    ///
    /// Rule: cr-dotnet-0045 — Session State Provider
    /// Remediation: Replaced InProc HttpSessionState with Azure Cache for Redis via IDistributedCache.
    /// Session value (idoriginal) is stored in and retrieved from Redis using RedisSessionHelper,
    /// enabling distributed session state that scales across multiple container instances without server affinity.
    ///
    /// Migration: System.Web.UI.Page -> Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    /// Migration: System.Web.UI / System.Web.UI.WebControls -> ASP.NET Core Razor Pages model binding
    /// Migration: asp:TextBox (loginEmail, loginPassword) -> [BindProperty] LoginEmail, LoginPassword
    /// Migration: asp:TextBox (sName, sBirthDate, sEmail, sPassword, scPassword, Phone, Address) -> [BindProperty] model properties
    /// Migration: asp:Button OnClick="loginV" -> OnPostLogin() handler
    /// Migration: asp:Button OnClick="signupV" -> OnPostSignup() handler
    /// Migration: Session["idoriginal"] = "" (InProc) -> RedisSessionHelper.SetInt32(_cache, "idoriginal", 0) (Azure Cache for Redis)
    /// Migration: Session["idoriginal"] = id (InProc) -> RedisSessionHelper.SetInt32(_cache, "idoriginal", id) (Azure Cache for Redis)
    /// Migration: Response.Redirect("~/Patient/PatientHome.aspx") -> RedirectToPage("/Patient/PatientHome")
    /// Migration: Response.Write("<script>alert(...)") -> ErrorMessage model property
    /// Migration: Request.Form["Gender"] -> [BindProperty] Gender
    /// Migration: IsPostBack check -> OnGet() / OnPostLogin() / OnPostSignup() separation in Razor Pages
    /// </summary>
    public class SignUpModel : PageModel
    {
        private readonly IDistributedCache _cache;

        public SignUpModel(IDistributedCache cache)
        {
            _cache = cache;
        }

        // Login form properties
        [BindProperty]
        public string LoginEmail { get; set; } = string.Empty;

        [BindProperty]
        public string LoginPassword { get; set; } = string.Empty;

        // Sign-up form properties
        [BindProperty]
        public string SName { get; set; } = string.Empty;

        [BindProperty]
        public string SBirthDate { get; set; } = string.Empty;

        [BindProperty]
        public string SEmail { get; set; } = string.Empty;

        [BindProperty]
        public string SPassword { get; set; } = string.Empty;

        [BindProperty]
        public string ScPassword { get; set; } = string.Empty;

        [BindProperty]
        public string Phone { get; set; } = string.Empty;

        [BindProperty]
        public string Gender { get; set; } = string.Empty;

        [BindProperty]
        public string Address { get; set; } = string.Empty;

        // Error/status message to display in the view
        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            // cr-dotnet-0045: Replaced HttpContext.Session.SetInt32 (InProc) with Redis-backed IDistributedCache
            RedisSessionHelper.SetInt32(_cache, "idoriginal", 0);
        }

        /// <summary>
        /// Handles the Login form submission.
        /// Replaces loginV(object sender, EventArgs e) from Web Forms code-behind.
        /// </summary>
        public IActionResult OnPostLogin()
        {
            var objmyDAl = new myDAL();

            int type = 0;
            int id = 0;

            int status = objmyDAl.validateLogin(LoginEmail, LoginPassword, ref type, ref id);

            if (status == 0)
            {
                // cr-dotnet-0045: Replaced HttpContext.Session.SetInt32 (InProc) with Redis-backed IDistributedCache
                RedisSessionHelper.SetInt32(_cache, "idoriginal", id);

                if (type == 1)
                {
                    return RedirectToPage("/Patient/PatientHome");
                }
                else if (type == 2)
                {
                    return RedirectToPage("/Doctor/DoctorHome");
                }
                else if (type == 3)
                {
                    return RedirectToPage("/Admin/AdminHome");
                }
            }
            else if (status == 1)
            {
                ErrorMessage = "Email not found. Try Again !";
            }
            else if (status == 2)
            {
                ErrorMessage = "Incorrect Password. Try Again !";
            }
            else if (status == -1)
            {
                ErrorMessage = "There was some error. Try Again !";
            }

            return Page();
        }

        /// <summary>
        /// Handles the Sign-Up form submission.
        /// Replaces signupV(object sender, EventArgs e) from Web Forms code-behind.
        /// </summary>
        public IActionResult OnPostSignup()
        {
            var objmyDAl = new myDAL();

            int id = 0;

            int status = objmyDAl.validateUser(SName, SBirthDate, SEmail, SPassword, Phone, Gender, Address, ref id);

            // status == 0 failure (email already exists)
            if (status == 0)
            {
                ErrorMessage = "Email already exists. Please choose a different one.";
            }
            else if (status == 1)
            {
                // cr-dotnet-0045: Replaced HttpContext.Session.SetInt32 (InProc) with Redis-backed IDistributedCache
                RedisSessionHelper.SetInt32(_cache, "idoriginal", id);
                return RedirectToPage("/Patient/PatientHome");
            }
            else if (status == -1)
            {
                ErrorMessage = "There was some error. Try again !";
            }

            return Page();
        }
    }
}
