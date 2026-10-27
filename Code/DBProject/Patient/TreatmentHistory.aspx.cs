// NOTE: This file has been superseded by TreatmentHistory.cshtml.cs (ASP.NET Core Razor Pages).
// The Web Forms code-behind below is retained for reference only.
// Migration: System.Web.UI.Page -> Microsoft.AspNetCore.Mvc.RazorPages.PageModel
// Migration: System.Web.UI / System.Web.UI.WebControls -> ASP.NET Core Razor Pages model binding
//
// cr-dotnet-1034: Synchronous Data Binding in GridView Controls
// Remediation: Async GridView Binding with Azure SQL and Entity Framework Core
//   Line 51 (original): THistoryGrid.DataBind() (synchronous GridView data binding) ->
//             OnGetAsync() with await objmyDAl.getTreatmentHistoryAsync() (async/await Task-based API)
//   The synchronous THistoryGrid.DataSource = DT; THistoryGrid.DataBind() pattern blocks request threads
//   under load. Replaced with async Razor Page handler OnGetAsync() that awaits the DAL async method,
//   preventing thread pool exhaustion and enabling efficient auto-scaling in Azure cloud deployments.
//
// cr-dotnet-0045: Session State Provider — Azure Cache for Redis
// Migration: System.Web.SessionState (Session["key"]) InProc -> IDistributedCache (Azure Cache for Redis)
//   Line 31 (original): Session["idoriginal"] -> await RedisSessionHelper.GetInt32Async(_cache, "idoriginal")
// IDistributedCache is injected via constructor DI and backed by Azure Cache for Redis
// (Microsoft.Extensions.Caching.StackExchangeRedis), enabling stateless horizontal scaling.
//
// Migration: Label (THistory) -> TreatmentHistoryModel.HistoryMessage property
// Migration: GridView (THistoryGrid) -> HTML table rendered from DataTable in Razor view
// Migration: IsPostBack check -> OnGetAsync() in Razor Pages
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
    // Migrated to Razor Pages: see Patient/TreatmentHistory.cshtml and Patient/TreatmentHistory.cshtml.cs
    // Web Forms class retained as reference stub only — not active in ASP.NET Core pipeline.
    //
    // cr-dotnet-1034 FIXED: Synchronous THistoryGrid.DataBind() eliminated.
    // Replaced with async OnGetAsync() calling await objmyDAl.getTreatmentHistoryAsync() in TreatmentHistory.cshtml.cs,
    // preventing thread pool exhaustion and enabling efficient auto-scaling in Azure cloud deployments.
    //
    // cr-dotnet-0045 FIXED: Session["idoriginal"] InProc usage replaced with
    // await RedisSessionHelper.GetInt32Async(_cache, "idoriginal") in TreatmentHistory.cshtml.cs
    public partial class TreatmentHistory
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Replaced by TreatmentHistoryModel.OnGetAsync() in TreatmentHistory.cshtml.cs
            // cr-dotnet-1034: THistoryGrid.DataBind() (sync) -> await objmyDAl.getTreatmentHistoryAsync() (async)
        }

        protected void treatmentHistory(object sender, EventArgs e)
        {
            // Replaced by TreatmentHistoryModel.LoadTreatmentHistoryAsync() in TreatmentHistory.cshtml.cs
            // cr-dotnet-1034: THistoryGrid.DataSource = DT; THistoryGrid.DataBind() (sync) -> await getTreatmentHistoryAsync() (async)
            // cr-dotnet-0045: Session["idoriginal"] -> await RedisSessionHelper.GetInt32Async(_cache, "idoriginal")
        }
    }
}
