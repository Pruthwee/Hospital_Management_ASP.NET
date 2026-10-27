// NOTE: This file has been superseded by PatientFeedback.cshtml.cs (ASP.NET Core Razor Pages).
// The Web Forms code-behind below is retained for reference only.
// Migration: System.Web.UI.Page -> Microsoft.AspNetCore.Mvc.RazorPages.PageModel
// Migration: System.Web.UI / System.Web.UI.WebControls -> ASP.NET Core Razor Pages model binding
//
// cr-dotnet-0126: Heavy Coupling to Stateful Middleware
// Remediation: Externalize session state to Azure Cache for Redis and refactor to HTTP APIs
//   Line 21: Session["aID"] = "" (IIS InProc sticky session) ->
//             RedisSessionHelper.SetString(_cache, "aID", string.Empty) (Azure Cache for Redis via IDistributedCache)
//   Line 56: Session["aID"] = aID (IIS InProc sticky session) ->
//             RedisSessionHelper.SetInt32(_cache, "aID", aID) (Azure Cache for Redis via IDistributedCache)
// IDistributedCache is injected via constructor DI and backed by Azure Cache for Redis
// (Microsoft.Extensions.Caching.StackExchangeRedis), enabling stateless horizontal scaling
// across AKS pods without session affinity / sticky sessions.
//
// cr-dotnet-0045: Session State Provider — Azure Cache for Redis
// Migration: System.Web.SessionState (Session["key"]) InProc -> IDistributedCache (Azure Cache for Redis)
//   Line 21: Session["aID"] =       -> RedisSessionHelper.SetString(_cache, "aID", string.Empty)
//   Line 35: Session["idoriginal"]  -> RedisSessionHelper.GetInt32(_cache, "idoriginal")
//   Line 56: Session["aID"] =       -> RedisSessionHelper.SetInt32(_cache, "aID", aID)
//   Line 79: Session["aID"]         -> RedisSessionHelper.GetInt32(_cache, "aID")
// IDistributedCache is injected via constructor DI and backed by Azure Cache for Redis
// (Microsoft.Extensions.Caching.StackExchangeRedis), enabling stateless horizontal scaling.
//
// Migration: Label (Feedback, FDoctor, FTimings, Message, F) -> PatientFeedbackModel properties
// Migration: DropDownList (List) -> HTML select element with [BindProperty] Rating
// Migration: asp:Button OnClick="giveFeedback" -> OnPostGiveFeedback() handler in PatientFeedback.cshtml.cs
// Migration: IsPostBack check -> OnGet() / OnPost() separation in Razor Pages
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
    // Migrated to Razor Pages: see Patient/PatientFeedback.cshtml and Patient/PatientFeedback.cshtml.cs
    // Web Forms class retained as reference stub only — not active in ASP.NET Core pipeline.
    //
    // cr-dotnet-0126 FIXED: IIS InProc sticky-session coupling eliminated.
    // Session["aID"] and Session["idoriginal"] replaced with Azure Cache for Redis
    // via RedisSessionHelper in PatientFeedback.cshtml.cs, enabling stateless horizontal
    // scaling on AKS without session affinity.
    public partial class PatientFeedback
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Replaced by PatientFeedbackModel.OnGet() in PatientFeedback.cshtml.cs
            // cr-dotnet-0126 / cr-dotnet-0045:
            //   Session["aID"] = "" (InProc) -> RedisSessionHelper.SetString(_cache, "aID", string.Empty) (Redis)
        }

        protected void pendingFeedback(object sender, EventArgs e)
        {
            // Replaced by PatientFeedbackModel.LoadPendingFeedback() in PatientFeedback.cshtml.cs
            // cr-dotnet-0126 / cr-dotnet-0045:
            //   Session["idoriginal"] (InProc) -> RedisSessionHelper.GetInt32(_cache, "idoriginal") (Redis)
            //   Session["aID"] = aID  (InProc) -> RedisSessionHelper.SetInt32(_cache, "aID", aID) (Redis)
        }

        protected void giveFeedback(object sender, EventArgs e)
        {
            // Replaced by PatientFeedbackModel.OnPostGiveFeedback() in PatientFeedback.cshtml.cs
            // cr-dotnet-0126 / cr-dotnet-0045:
            //   Session["aID"] (InProc) -> RedisSessionHelper.GetInt32(_cache, "aID") (Redis)
        }
    }
}
