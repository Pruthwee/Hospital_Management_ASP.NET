// NOTE: This file has been superseded by SignUp.cshtml.cs (ASP.NET Core Razor Pages).
// The Web Forms code-behind below is retained for reference only.
// Migration: System.Web.UI.Page -> Microsoft.AspNetCore.Mvc.RazorPages.PageModel
// Migration: System.Web.UI / System.Web.UI.WebControls -> ASP.NET Core Razor Pages model binding
//
// cr-dotnet-0126: Heavy Coupling to Stateful Middleware
// Remediation: Externalize session state to Azure Cache for Redis and refactor to HTTP APIs
//   Line 17:  Session["idoriginal"] = "" (IIS InProc sticky session) ->
//              RedisSessionHelper.SetInt32(_cache, "idoriginal", 0) (Azure Cache for Redis via IDistributedCache)
//   Line 36:  Session["idoriginal"] = id (IIS InProc sticky session) ->
//              RedisSessionHelper.SetInt32(_cache, "idoriginal", id) (Azure Cache for Redis via IDistributedCache)
//   Line 109: Session["idoriginal"] = id (IIS InProc sticky session) ->
//              RedisSessionHelper.SetInt32(_cache, "idoriginal", id) (Azure Cache for Redis via IDistributedCache)
// IDistributedCache is injected via constructor DI and backed by Azure Cache for Redis
// (Microsoft.Extensions.Caching.StackExchangeRedis), enabling stateless horizontal scaling
// across AKS pods without session affinity / sticky sessions.
//
// cr-dotnet-0045: Session State Provider — Azure Cache for Redis
// Migration: System.Web.SessionState (Session["key"]) InProc -> IDistributedCache (Azure Cache for Redis)
//   Line 17 (original): Session["idoriginal"] = "" -> RedisSessionHelper.SetInt32(_cache, "idoriginal", 0)
//   Line 36 (original): Session["idoriginal"] = id -> RedisSessionHelper.SetInt32(_cache, "idoriginal", id)
//   Line 109 (original): Session["idoriginal"] = id -> RedisSessionHelper.SetInt32(_cache, "idoriginal", id)
// IDistributedCache is injected via constructor DI and backed by Azure Cache for Redis
// (Microsoft.Extensions.Caching.StackExchangeRedis), enabling stateless horizontal scaling.
//
// Migration: asp:TextBox (loginEmail, loginPassword) -> [BindProperty] LoginEmail, LoginPassword
// Migration: asp:TextBox (sName, sBirthDate, sEmail, sPassword, scPassword, Phone, Address) -> [BindProperty] model properties
// Migration: asp:Button OnClick="loginV" -> OnPostLogin() handler
// Migration: asp:Button OnClick="signupV" -> OnPostSignup() handler
// Migration: Response.Redirect("~/Patient/PatientHome.aspx") -> RedirectToPage("/Patient/PatientHome")
// Migration: Response.Write("<script>alert(...)") -> ErrorMessage model property rendered in view
// Migration: Request.Form["Gender"] -> [BindProperty] Gender model property
// Migration: IsPostBack check -> OnGet() / OnPostLogin() / OnPostSignup() separation in Razor Pages
// ViewState and postback patterns replaced with stateless Razor Page handler methods.
// using System.Web;                  // REMOVED: Not available in ASP.NET Core
// using System.Web.UI;               // REMOVED: Web Forms Page base class not used in Razor Pages
// using System.Web.UI.WebControls;   // REMOVED: Server controls replaced with HTML + Tag Helpers
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using DBProject.DAL;

namespace DBProject
{
    // Migrated to Razor Pages: see SignUp.cshtml and SignUp.cshtml.cs
    // Web Forms class retained as reference stub only — not active in ASP.NET Core pipeline.
    //
    // cr-dotnet-0126 FIXED: IIS InProc sticky-session coupling eliminated.
    // Session["idoriginal"] replaced with Azure Cache for Redis via RedisSessionHelper
    // in SignUp.cshtml.cs, enabling stateless horizontal scaling on AKS without session affinity.
    //
    // cr-dotnet-0045 FIXED: All Session["idoriginal"] InProc usages replaced with
    // RedisSessionHelper.SetInt32(_cache, "idoriginal", ...) in SignUp.cshtml.cs
    public partial class SignUp
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Replaced by SignUpModel.OnGet() in SignUp.cshtml.cs
            // cr-dotnet-0126 / cr-dotnet-0045:
            //   Session["idoriginal"] = "" (InProc) -> RedisSessionHelper.SetInt32(_cache, "idoriginal", 0) (Redis)
        }

        protected void loginV(object sender, EventArgs e)
        {
            // Replaced by SignUpModel.OnPostLogin() in SignUp.cshtml.cs
            // cr-dotnet-0126 / cr-dotnet-0045:
            //   Session["idoriginal"] = id (InProc) -> RedisSessionHelper.SetInt32(_cache, "idoriginal", id) (Redis)
        }

        protected void signupV(object sender, EventArgs e)
        {
            // Replaced by SignUpModel.OnPostSignup() in SignUp.cshtml.cs
            // cr-dotnet-0126 / cr-dotnet-0045:
            //   Session["idoriginal"] = id (InProc) -> RedisSessionHelper.SetInt32(_cache, "idoriginal", id) (Redis)
        }
    }
}
