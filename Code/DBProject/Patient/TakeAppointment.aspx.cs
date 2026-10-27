// NOTE: This file has been superseded by TakeAppointment.cshtml.cs (ASP.NET Core Razor Pages).
// The Web Forms code-behind below is retained for reference only.
// Migration: System.Web.UI.Page -> Microsoft.AspNetCore.Mvc.RazorPages.PageModel
// Migration: System.Web.UI / System.Web.UI.WebControls -> ASP.NET Core Razor Pages model binding
//
// cr-dotnet-1034: Synchronous Data Binding in GridView Controls
// Remediation: Async GridView Binding with Azure SQL and Entity Framework Core
//   Line 63 (original): TDeptGrid.DataBind() (synchronous GridView data binding) ->
//             OnGetAsync() with await objmyDAl.getdeptInfoAsync() (async/await Task-based API)
//   The synchronous TDeptGrid.DataSource = DT; TDeptGrid.DataBind() pattern blocks request threads
//   under load. Replaced with async Razor Page handler OnGetAsync() that awaits the DAL async method,
//   preventing thread pool exhaustion and enabling efficient auto-scaling in Azure cloud deployments.
//
// cr-dotnet-0126: Heavy Coupling to Stateful Middleware
// Remediation: Externalize session state to Azure Cache for Redis and refactor to HTTP APIs
//   Line 17: Session["deptOriginal"] = "" (IIS InProc sticky session) ->
//             await RedisSessionHelper.SetStringAsync(_cache, "deptOriginal", string.Empty) (Azure Cache for Redis)
//   Line 31: Session["deptOriginal"] = deptName (IIS InProc sticky session) ->
//             await RedisSessionHelper.SetStringAsync(_cache, "deptOriginal", DeptName) (Azure Cache for Redis)
//
// cr-dotnet-0045: Session State Provider — Azure Cache for Redis
// Migration: System.Web.SessionState (Session["key"]) InProc -> IDistributedCache (Azure Cache for Redis)
//   Line 17 (original): Session["deptOriginal"] = "" -> await RedisSessionHelper.SetStringAsync(_cache, "deptOriginal", string.Empty)
//   Line 31 (original): Session["deptOriginal"] = deptName -> await RedisSessionHelper.SetStringAsync(_cache, "deptOriginal", DeptName)
// IDistributedCache is injected via constructor DI and backed by Azure Cache for Redis
// (Microsoft.Extensions.Caching.StackExchangeRedis), enabling stateless horizontal scaling.
//
// Migration: Label (TDept) -> TakeAppointmentModel.DeptMessage property
// Migration: GridView (TDeptGrid) with OnRowCommand -> HTML table with async form POST handler OnPostSelectDepartmentAsync()
// Migration: Response.Redirect("ViewDoctors.aspx") -> RedirectToPage("ViewDoctors")
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
    // Migrated to Razor Pages: see Patient/TakeAppointment.cshtml and Patient/TakeAppointment.cshtml.cs
    // Web Forms class retained as reference stub only — not active in ASP.NET Core pipeline.
    //
    // cr-dotnet-1034 FIXED: Synchronous TDeptGrid.DataBind() eliminated.
    // Replaced with async OnGetAsync() calling await objmyDAl.getdeptInfoAsync() in TakeAppointment.cshtml.cs,
    // preventing thread pool exhaustion and enabling efficient auto-scaling in Azure cloud deployments.
    //
    // cr-dotnet-0126 FIXED: IIS InProc sticky-session coupling eliminated.
    // Session["deptOriginal"] replaced with Azure Cache for Redis via RedisSessionHelper
    // in TakeAppointment.cshtml.cs, enabling stateless horizontal scaling on AKS without session affinity.
    //
    // cr-dotnet-0045 FIXED: All Session["deptOriginal"] InProc usages replaced with
    // await RedisSessionHelper.SetStringAsync(_cache, "deptOriginal", ...) in TakeAppointment.cshtml.cs
    public partial class TakeAppointment
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Replaced by TakeAppointmentModel.OnGetAsync() in TakeAppointment.cshtml.cs
            // cr-dotnet-1034: TDeptGrid.DataBind() (sync) -> await objmyDAl.getdeptInfoAsync() (async)
            // cr-dotnet-0126 / cr-dotnet-0045:
            //   Session["deptOriginal"] = "" (InProc) -> await RedisSessionHelper.SetStringAsync(_cache, "deptOriginal", string.Empty) (Redis)
        }

        protected void TDeptGrid_RowCommand(object sender, EventArgs e)
        {
            // Replaced by TakeAppointmentModel.OnPostSelectDepartmentAsync() in TakeAppointment.cshtml.cs
            // cr-dotnet-0126 / cr-dotnet-0045:
            //   Session["deptOriginal"] = deptName (InProc) -> await RedisSessionHelper.SetStringAsync(_cache, "deptOriginal", DeptName) (Redis)
        }

        protected void deptInfo(object sender, EventArgs e)
        {
            // Replaced by TakeAppointmentModel.LoadDeptInfoAsync() in TakeAppointment.cshtml.cs
            // cr-dotnet-1034: TDeptGrid.DataSource = DT; TDeptGrid.DataBind() (sync) -> await getdeptInfoAsync() (async)
        }
    }
}
