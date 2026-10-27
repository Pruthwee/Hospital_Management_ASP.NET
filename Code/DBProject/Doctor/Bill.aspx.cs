// NOTE: This file has been superseded by Bill.cshtml.cs (ASP.NET Core Razor Pages).
// The Web Forms code-behind below is retained for reference only.
// Migration: System.Web.UI.Page -> Microsoft.AspNetCore.Mvc.RazorPages.PageModel
// Migration: System.Web.UI / System.Web.UI.WebControls -> ASP.NET Core Razor Pages model binding
//
// cr-dotnet-0045: Session State Provider — Azure Cache for Redis
// Migration: System.Web.SessionState (Session["key"]) InProc -> IDistributedCache (Azure Cache for Redis)
//   Line 21: Session["idoriginal"]  -> RedisSessionHelper.GetInt32(_cache, "idoriginal")
//   Line 38: Session["idoriginal"]  -> RedisSessionHelper.GetInt32(_cache, "idoriginal")
//   Line 39: Session["appointid"]   -> RedisSessionHelper.GetInt32(_cache, "appointid")
//   Line 51: Session["idoriginal"]  -> RedisSessionHelper.GetInt32(_cache, "idoriginal")
//   Line 52: Session["appointid"]   -> RedisSessionHelper.GetInt32(_cache, "appointid")
// IDistributedCache is injected via constructor DI and backed by Azure Cache for Redis
// (Microsoft.Extensions.Caching.StackExchangeRedis), enabling stateless horizontal scaling.
//
// ViewState and postback patterns replaced with stateless Razor Page handler methods.
// using System.Web;           // REMOVED: Not available in ASP.NET Core
// using System.Web.UI;        // REMOVED: Web Forms Page base class not used in Razor Pages
// using System.Web.UI.WebControls; // REMOVED: Server controls replaced with HTML + Tag Helpers
using System;
using System.Data;
using DBProject.DAL;

namespace doctor
{
    // Migrated to Razor Pages: see Doctor/Bill.cshtml and Doctor/Bill.cshtml.cs
    // Web Forms class retained as reference stub only — not active in ASP.NET Core pipeline.
    public partial class bill
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Replaced by BillModel.OnGet() in Bill.cshtml.cs
            // cr-dotnet-0045: Session["idoriginal"] -> RedisSessionHelper.GetInt32(_cache, "idoriginal")
        }

        public void bill_paid(object sender, EventArgs e)
        {
            // Replaced by BillModel.OnPostBillPaid() in Bill.cshtml.cs
            // cr-dotnet-0045: Session["idoriginal"] -> RedisSessionHelper.GetInt32(_cache, "idoriginal")
            // cr-dotnet-0045: Session["appointid"]  -> RedisSessionHelper.GetInt32(_cache, "appointid")
        }

        public void bill_Unpaid(object sender, EventArgs e)
        {
            // Replaced by BillModel.OnPostBillUnpaid() in Bill.cshtml.cs
            // cr-dotnet-0045: Session["idoriginal"] -> RedisSessionHelper.GetInt32(_cache, "idoriginal")
            // cr-dotnet-0045: Session["appointid"]  -> RedisSessionHelper.GetInt32(_cache, "appointid")
        }
    }
}
