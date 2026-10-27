// NOTE: This file has been superseded by PendingAppointment.cshtml.cs (ASP.NET Core Razor Pages).
// The Web Forms code-behind below is retained for reference only.
// Migration: System.Web.UI.Page -> Microsoft.AspNetCore.Mvc.RazorPages.PageModel
// Migration: System.Web.UI / System.Web.UI.WebControls -> ASP.NET Core Razor Pages model binding
//
// cr-dotnet-0045: Session State Provider — Azure Cache for Redis
// Migration: System.Web.SessionState (Session["key"]) InProc -> IDistributedCache (Azure Cache for Redis)
//   Line 25: Session["idoriginal"] -> await RedisSessionHelper.GetInt32Async(_cache, "idoriginal")
// IDistributedCache is injected via constructor DI and backed by Azure Cache for Redis
// (Microsoft.Extensions.Caching.StackExchangeRedis), enabling stateless horizontal scaling.
//
// cr-dotnet-1034: Synchronous Data Binding in GridView Controls
// Remediation: Async GridView Binding with Azure SQL and Entity Framework Core
//   Line 32: pendingappointments.DataSource = dt; pendingappointments.DataBind() (synchronous) ->
//             PendingAppointmentModel.OnGetAsync() with await GetAllpendingappointmentsAsync() (async/await)
//   Async DAL method GetAllpendingappointmentsAsync() uses SqlCommand.ExecuteNonQueryAsync() and async
//   connection open, preventing thread pool exhaustion under load in Azure cloud deployments.
//   Connection strings stored in Azure Key Vault and resolved via environment variables.
//
// Migration: GridView (pendingappointments) with OnRowCommand -> OnPostApproveAppointmentAsync() handler
// Migration: GridView OnRowDeleting -> OnPostDeleteAppointmentAsync() handler
// Migration: GridViewCommandEventArgs / GridViewDeleteEventArgs -> [BindProperty] AppointmentId
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
    // Migrated to Razor Pages: see Doctor/PendingAppointment.cshtml and Doctor/PendingAppointment.cshtml.cs
    // Web Forms class retained as reference stub only — not active in ASP.NET Core pipeline.
    // cr-dotnet-1034 FIXED: Synchronous GridView data binding eliminated.
    // OnGet() replaced with async OnGetAsync() using GetAllpendingappointmentsAsync(), enabling
    // efficient auto-scaling in Azure cloud deployments.
    public partial class pendingappointment
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Replaced by PendingAppointmentModel.OnGetAsync() in PendingAppointment.cshtml.cs
            // cr-dotnet-1034: pendingappointments.DataBind() (sync) -> await GetAllpendingappointmentsAsync() (async)
            // cr-dotnet-0045: Session["idoriginal"] -> await RedisSessionHelper.GetInt32Async(_cache, "idoriginal")
        }

        public void loadgrid()
        {
            // Replaced by PendingAppointmentModel.LoadGridAsync() in PendingAppointment.cshtml.cs
        }

        protected void update_appointment(object sender, object e)
        {
            // Replaced by PendingAppointmentModel.OnPostApproveAppointmentAsync() in PendingAppointment.cshtml.cs
        }

        protected void Delete_appointment(object sender, object e)
        {
            // Replaced by PendingAppointmentModel.OnPostDeleteAppointmentAsync() in PendingAppointment.cshtml.cs
        }
    }
}
