// NOTE: This file has been superseded by HistoryUpdate.cshtml.cs (ASP.NET Core Razor Pages).
// The Web Forms code-behind below is retained for reference only.
// Migration: System.Web.UI.Page -> Microsoft.AspNetCore.Mvc.RazorPages.PageModel
// Migration: System.Web.UI / System.Web.UI.WebControls -> ASP.NET Core Razor Pages model binding
//
// cr-dotnet-0045: Session State Provider — Azure Cache for Redis
// Migration: System.Web.SessionState (Session["key"]) InProc -> IDistributedCache (Azure Cache for Redis)
//   Line 23: Session["idoriginal"] -> RedisSessionHelper.GetInt32(_cache, "idoriginal")
//   Line 28: Session["appointid"]  -> RedisSessionHelper.GetInt32(_cache, "appointid")
// IDistributedCache is injected via constructor DI and backed by Azure Cache for Redis
// (Microsoft.Extensions.Caching.StackExchangeRedis), enabling stateless horizontal scaling.
//
// Migration: TextBox server controls (Disease.Text, progress.Text, Prescription.Text) -> [BindProperty] model properties
// Migration: Button OnClick postbacks (saveindatabase, generate_bill) -> OnPostSaveInDatabase() / OnPostGenerateBill()
// Migration: Response.Redirect("bill.aspx") -> RedirectToPage("Bill")
// Migration: Response.Write("<script>alert(...);</script>") -> TempData["Error"] / TempData["Success"]
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
    // Migrated to Razor Pages: see Doctor/HistoryUpdate.cshtml and Doctor/HistoryUpdate.cshtml.cs
    // Web Forms class retained as reference stub only — not active in ASP.NET Core pipeline.
    public partial class Historyupdate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Replaced by HistoryUpdateModel.OnGet() in HistoryUpdate.cshtml.cs
        }

        public void saveindatabase(object sender, EventArgs e)
        {
            // Replaced by HistoryUpdateModel.OnPostSaveInDatabase() in HistoryUpdate.cshtml.cs
            // cr-dotnet-0045: Session["idoriginal"] -> RedisSessionHelper.GetInt32(_cache, "idoriginal")
            // cr-dotnet-0045: Session["appointid"]  -> RedisSessionHelper.GetInt32(_cache, "appointid")
        }

        public void generate_bill(object sender, EventArgs e)
        {
            // Replaced by HistoryUpdateModel.OnPostGenerateBill() in HistoryUpdate.cshtml.cs
        }
    }
}
