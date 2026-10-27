// NOTE: This file has been superseded by PatientHistory.cshtml.cs (ASP.NET Core Razor Pages).
// The Web Forms code-behind below is retained for reference only.
// Migration: System.Web.UI.Page -> Microsoft.AspNetCore.Mvc.RazorPages.PageModel
// Migration: System.Web.UI / System.Web.UI.WebControls -> ASP.NET Core Razor Pages model binding
//
// cr-dotnet-0126: Heavy Coupling to Stateful Middleware
// Remediation: Externalize session state to Azure Cache for Redis and refactor to HTTP APIs
//   Line 46: Session["idoriginal"] (IIS InProc sticky session) ->
//             await RedisSessionHelper.GetInt32Async(_cache, "idoriginal") (Azure Cache for Redis via IDistributedCache)
//   Line 46: Session["appointid"] = appointmentid (IIS InProc sticky session) ->
//             await RedisSessionHelper.SetInt32Async(_cache, "appointid", appointmentid) (Azure Cache for Redis via IDistributedCache)
// IDistributedCache is injected via constructor DI and backed by Azure Cache for Redis
// (Microsoft.Extensions.Caching.StackExchangeRedis), enabling stateless horizontal scaling
// across AKS pods without session affinity / sticky sessions.
//
// cr-dotnet-0045: Session State Provider — Azure Cache for Redis
// Migration: System.Web.SessionState (Session["key"]) InProc -> IDistributedCache (Azure Cache for Redis)
//   Line 46: Session["idoriginal"] -> await RedisSessionHelper.GetInt32Async(_cache, "idoriginal")
//   Line 46: Session["appointid"] = -> await RedisSessionHelper.SetInt32Async(_cache, "appointid", appointmentid)
//
// cr-dotnet-1034: Synchronous Data Binding in GridView Controls
// Remediation: Async GridView Binding with Azure SQL and Entity Framework Core
//   Line 29: patientsgrid.DataSource = dt; patientsgrid.DataBind() (synchronous) ->
//             PatientHistoryModel.OnGetAsync() with await search_patient_DALAsync() (async/await)
//   Async DAL method search_patient_DALAsync() uses SqlCommand.ExecuteNonQueryAsync() and async
//   connection open, preventing thread pool exhaustion under load in Azure cloud deployments.
//   Connection strings stored in Azure Key Vault and resolved via environment variables.
//
// Migration: GridView (patientsgrid) with AutoGenerateSelectButton / OnRowCommand -> HTML table with named POST handler
// Migration: GridViewCommandEventArgs -> [BindProperty] AppointmentId
// Migration: Response.Redirect -> RedirectToPage()
// Migration: Response.Write("<script>alert(...)") -> TempData["Error"] model property rendered in view
// ViewState and postback patterns replaced with stateless Razor Page handler methods.
// using System.Web;                  // REMOVED: Not available in ASP.NET Core
// using System.Web.UI;               // REMOVED: Web Forms Page base class not used in Razor Pages
// using System.Web.UI.WebControls;   // REMOVED: Server controls replaced with HTML + Tag Helpers
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using DBProject.DAL;

namespace doctor
{
    // Migrated to Razor Pages: see Doctor/PatientHistory.cshtml and Doctor/PatientHistory.cshtml.cs
    // Web Forms class retained as reference stub only — not active in ASP.NET Core pipeline.
    //
    // cr-dotnet-0126 FIXED: IIS InProc sticky-session coupling eliminated.
    // cr-dotnet-1034 FIXED: Synchronous GridView data binding eliminated.
    // Session["idoriginal"] and Session["appointid"] replaced with Azure Cache for Redis
    // via RedisSessionHelper in PatientHistory.cshtml.cs, enabling stateless horizontal
    // scaling on AKS without session affinity.
    // OnGet() replaced with async OnGetAsync() using search_patient_DALAsync(), enabling
    // efficient auto-scaling in Azure cloud deployments.
    public partial class patienthistory
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Replaced by PatientHistoryModel.OnGetAsync() in PatientHistory.cshtml.cs
            // cr-dotnet-1034: patientsgrid.DataBind() (sync) -> await search_patient_DALAsync() (async)
            // cr-dotnet-0126 / cr-dotnet-0045:
            //   Session["idoriginal"] (InProc) -> await RedisSessionHelper.GetInt32Async(_cache, "idoriginal") (Redis)
        }

        protected void patientsgrid_RowCommand(object sender, EventArgs e)
        {
            // Replaced by PatientHistoryModel.OnPostSelectPatientAsync() in PatientHistory.cshtml.cs
            // cr-dotnet-0126 / cr-dotnet-0045:
            //   Session["appointid"] = appointmentid (InProc) -> await RedisSessionHelper.SetInt32Async(_cache, "appointid", AppointmentId) (Redis)
        }
    }
}
