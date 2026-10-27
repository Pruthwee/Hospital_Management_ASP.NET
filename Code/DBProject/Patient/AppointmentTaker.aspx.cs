// NOTE: This file has been superseded by AppointmentTaker.cshtml.cs (ASP.NET Core Razor Pages).
// The Web Forms code-behind below is retained for reference only.
// Migration: System.Web.UI.Page -> Microsoft.AspNetCore.Mvc.RazorPages.PageModel
// Migration: System.Web.UI / System.Web.UI.WebControls -> ASP.NET Core Razor Pages model binding
//
// cr-dotnet-0126: Heavy Coupling to Stateful Middleware
// Remediation: Externalize session state to Azure Cache for Redis and refactor to HTTP APIs
//   Line 17: Session["freeSlot"] = "" (IIS InProc sticky session) ->
//             await RedisSessionHelper.SetStringAsync(_cache, "freeSlot", string.Empty) (Azure Cache for Redis via IDistributedCache)
//   Line 33: Session["freeSlot"] = tokens[0] (IIS InProc sticky session) ->
//             await RedisSessionHelper.SetStringAsync(_cache, "freeSlot", tokens[0]) (Azure Cache for Redis via IDistributedCache)
// IDistributedCache is injected via constructor DI and backed by Azure Cache for Redis
// (Microsoft.Extensions.Caching.StackExchangeRedis), enabling stateless horizontal scaling
// across AKS pods without session affinity / sticky sessions.
//
// cr-dotnet-0045: Session State Provider — Azure Cache for Redis
// Migration: System.Web.SessionState (Session["key"]) InProc -> IDistributedCache (Azure Cache for Redis)
//   Line 17: Session["freeSlot"] =  -> await RedisSessionHelper.SetStringAsync(_cache, "freeSlot", string.Empty)
//   Line 33: Session["freeSlot"] =  -> await RedisSessionHelper.SetStringAsync(_cache, "freeSlot", tokens[0])
//   Line 52: Session["dID"]         -> await RedisSessionHelper.GetStringAsync(_cache, "dID")
//   Line 57: Session["idoriginal"]  -> await RedisSessionHelper.GetInt32Async(_cache, "idoriginal")
// IDistributedCache is injected via constructor DI and backed by Azure Cache for Redis
// (Microsoft.Extensions.Caching.StackExchangeRedis), enabling stateless horizontal scaling.
//
// cr-dotnet-1034: Synchronous Data Binding in GridView Controls
// Remediation: Async GridView Binding with Azure SQL and Entity Framework Core
//   Line 77: PAppointmentGrid.DataSource = dt; PAppointmentGrid.DataBind() (synchronous) ->
//             AppointmentTakerModel.OnGetAsync() with await getFreeSlotsAsync() (async/await)
//   Async DAL method getFreeSlotsAsync() uses SqlCommand.ExecuteNonQueryAsync() and async
//   connection open, preventing thread pool exhaustion under load in Azure cloud deployments.
//   Connection strings stored in Azure Key Vault and resolved via environment variables.
//
// Migration: GridView (PAppointmentGrid) with AutoGenerateSelectButton / OnRowCommand -> HTML table with named POST handler
// Migration: GridViewCommandEventArgs -> [BindProperty] SlotValue
// Migration: Response.Redirect -> RedirectToPage()
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
    // Migrated to Razor Pages: see Patient/AppointmentTaker.cshtml and Patient/AppointmentTaker.cshtml.cs
    // Web Forms class retained as reference stub only — not active in ASP.NET Core pipeline.
    //
    // cr-dotnet-0126 FIXED: IIS InProc sticky-session coupling eliminated.
    // cr-dotnet-1034 FIXED: Synchronous GridView data binding eliminated.
    // Session["freeSlot"], Session["dID"], and Session["idoriginal"] replaced with Azure Cache for Redis
    // via RedisSessionHelper in AppointmentTaker.cshtml.cs, enabling stateless horizontal
    // scaling on AKS without session affinity.
    // OnGet() replaced with async OnGetAsync() using getFreeSlotsAsync(), enabling
    // efficient auto-scaling in Azure cloud deployments.
    public partial class AppointmentTaker
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Replaced by AppointmentTakerModel.OnGetAsync() in AppointmentTaker.cshtml.cs
            // cr-dotnet-1034: PAppointmentGrid.DataBind() (sync) -> await getFreeSlotsAsync() (async)
            // cr-dotnet-0126 / cr-dotnet-0045:
            //   Session["freeSlot"] = "" (InProc) -> await RedisSessionHelper.SetStringAsync(_cache, "freeSlot", string.Empty) (Redis)
        }

        protected void PAppointmentGrid_RowCommand(object sender, EventArgs e)
        {
            // Replaced by AppointmentTakerModel.OnPostSelectSlotAsync() in AppointmentTaker.cshtml.cs
            // cr-dotnet-0126 / cr-dotnet-0045:
            //   Session["freeSlot"] = tokens[0] (InProc) -> await RedisSessionHelper.SetStringAsync(_cache, "freeSlot", tokens[0]) (Redis)
        }

        protected void freeSlots(object sender, EventArgs e)
        {
            // Replaced by AppointmentTakerModel.LoadFreeSlotsAsync() in AppointmentTaker.cshtml.cs
            // cr-dotnet-1034: PAppointmentGrid.DataBind() (sync) -> await getFreeSlotsAsync() (async)
            // cr-dotnet-0126 / cr-dotnet-0045:
            //   Session["dID"]        (InProc) -> await RedisSessionHelper.GetStringAsync(_cache, "dID") (Redis)
            //   Session["idoriginal"] (InProc) -> await RedisSessionHelper.GetInt32Async(_cache, "idoriginal") (Redis)
        }
    }
}
