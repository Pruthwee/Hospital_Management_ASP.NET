// NOTE: This file has been superseded by ViewDoctors.cshtml.cs (ASP.NET Core Razor Pages).
// The Web Forms code-behind below is retained for reference only.
// Migration: System.Web.UI.Page -> Microsoft.AspNetCore.Mvc.RazorPages.PageModel
// Migration: System.Web.UI / System.Web.UI.WebControls -> ASP.NET Core Razor Pages model binding
//
// cr-dotnet-1034: Synchronous Data Binding in GridView Controls
// Remediation: Async GridView Binding with Azure SQL and Entity Framework Core
//   Line 63 (original): TDoctorGrid.DataBind() (synchronous GridView data binding) ->
//             OnGetAsync() with await objmyDAl.getDeptDoctorInfoAsync() (async/await Task-based API)
//   The synchronous TDoctorGrid.DataSource = DT; TDoctorGrid.DataBind() pattern blocks request threads
//   under load. Replaced with async Razor Page handler OnGetAsync() that awaits the DAL async method,
//   preventing thread pool exhaustion and enabling efficient auto-scaling in Azure cloud deployments.
//
// cr-dotnet-0126: Heavy Coupling to Stateful Middleware
// Remediation: Externalize session state to Azure Cache for Redis and refactor to HTTP APIs
//   Line 17: Session["dID"] = "" (IIS InProc sticky session) ->
//             await RedisSessionHelper.SetStringAsync(_cache, "dID", string.Empty) (Azure Cache for Redis)
//   Line 30: Session["dID"] = dID (IIS InProc sticky session) ->
//             await RedisSessionHelper.SetStringAsync(_cache, "dID", DoctorId) (Azure Cache for Redis)
//
// cr-dotnet-0045: Session State Provider — Azure Cache for Redis
// Migration: System.Web.SessionState (Session["key"]) InProc -> IDistributedCache (Azure Cache for Redis)
//   Line 17 (original): Session["dID"] = "" -> await RedisSessionHelper.SetStringAsync(_cache, "dID", string.Empty)
//   Line 30 (original): Session["dID"] = dID -> await RedisSessionHelper.SetStringAsync(_cache, "dID", DoctorId)
//   Line 49 (original): Session["deptOriginal"] -> await RedisSessionHelper.GetStringAsync(_cache, "deptOriginal")
//   Line 61 (original): Session["deptOriginal"] -> await RedisSessionHelper.GetStringAsync(_cache, "deptOriginal")
// IDistributedCache is injected via constructor DI and backed by Azure Cache for Redis
// (Microsoft.Extensions.Caching.StackExchangeRedis), enabling stateless horizontal scaling.
//
// Migration: Label (TDoctor) -> ViewDoctorsModel.DoctorMessage property
// Migration: GridView (TDoctorGrid) with OnRowCommand -> HTML table with async form POST handler OnPostSelectDoctorAsync()
// Migration: Response.Redirect("DoctorProfile.aspx") -> RedirectToPage("DoctorProfile")
// Migration: IsPostBack check -> OnGetAsync() / OnPostAsync() separation in Razor Pages
// ViewState and postback patterns replaced with stateless async Razor Page handler methods.
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
    // Migrated to Razor Pages: see Patient/ViewDoctors.cshtml and Patient/ViewDoctors.cshtml.cs
    // Web Forms class retained as reference stub only — not active in ASP.NET Core pipeline.
    //
    // cr-dotnet-1034 FIXED: Synchronous TDoctorGrid.DataBind() eliminated.
    // Replaced with async OnGetAsync() calling await objmyDAl.getDeptDoctorInfoAsync() in ViewDoctors.cshtml.cs,
    // preventing thread pool exhaustion and enabling efficient auto-scaling in Azure cloud deployments.
    //
    // cr-dotnet-0126 FIXED: IIS InProc sticky-session coupling eliminated.
    // Session["dID"] and Session["deptOriginal"] replaced with Azure Cache for Redis
    // via RedisSessionHelper async methods in ViewDoctors.cshtml.cs, enabling stateless horizontal
    // scaling on AKS without session affinity.
    //
    // cr-dotnet-0045 FIXED: All Session["dID"] and Session["deptOriginal"] InProc usages replaced with
    // await RedisSessionHelper.SetStringAsync/_GetStringAsync(_cache, ...) in ViewDoctors.cshtml.cs
    public partial class ViewDoctors
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Replaced by ViewDoctorsModel.OnGetAsync() in ViewDoctors.cshtml.cs
            // cr-dotnet-1034: TDoctorGrid.DataBind() (sync) -> await objmyDAl.getDeptDoctorInfoAsync() (async)
            // cr-dotnet-0126 / cr-dotnet-0045:
            //   Session["dID"] = "" (InProc) -> await RedisSessionHelper.SetStringAsync(_cache, "dID", string.Empty) (Redis)
        }

        protected void TDoctorGrid_RowCommand(object sender, EventArgs e)
        {
            // Replaced by ViewDoctorsModel.OnPostSelectDoctorAsync() in ViewDoctors.cshtml.cs
            // cr-dotnet-0126 / cr-dotnet-0045:
            //   Session["dID"] = dID (InProc) -> await RedisSessionHelper.SetStringAsync(_cache, "dID", DoctorId) (Redis)
        }

        protected void deptDoctorInfo(object sender, EventArgs e)
        {
            // Replaced by ViewDoctorsModel.LoadDeptDoctorInfoAsync() in ViewDoctors.cshtml.cs
            // cr-dotnet-1034: TDoctorGrid.DataSource = DT; TDoctorGrid.DataBind() (sync) -> await getDeptDoctorInfoAsync() (async)
            // cr-dotnet-0126 / cr-dotnet-0045:
            //   Session["deptOriginal"] (InProc) -> await RedisSessionHelper.GetStringAsync(_cache, "deptOriginal") (Redis)
        }
    }
}
