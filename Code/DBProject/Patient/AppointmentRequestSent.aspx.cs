// NOTE: This file has been superseded by AppointmentRequestSent.cshtml.cs (ASP.NET Core Razor Pages).
// The Web Forms code-behind below is retained for reference only.
// Migration: System.Web.UI.Page -> Microsoft.AspNetCore.Mvc.RazorPages.PageModel
// Migration: System.Web.UI / System.Web.UI.WebControls -> ASP.NET Core Razor Pages model binding
//
// cr-dotnet-0045: Session State Provider — Azure Cache for Redis
// Migration: System.Web.SessionState (Session["key"]) InProc -> IDistributedCache (Azure Cache for Redis)
//   Line 28: Session["dID"]         -> RedisSessionHelper.GetString(_cache, "dID")
//   Line 33: Session["idoriginal"]  -> RedisSessionHelper.GetInt32(_cache, "idoriginal")
//   Line 36: Session["freeSlot"]    -> RedisSessionHelper.GetString(_cache, "freeSlot")
// IDistributedCache is injected via constructor DI and backed by Azure Cache for Redis
// (Microsoft.Extensions.Caching.StackExchangeRedis), enabling stateless horizontal scaling.
//
// Migration: asp:Button OnClick="sendARequest" -> OnPostSendRequest() handler in AppointmentRequestSent.cshtml.cs
// Migration: asp:Label (Message) for result text -> AppointmentRequestSentModel.Message property
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
    // Migrated to Razor Pages: see Patient/AppointmentRequestSent.cshtml and Patient/AppointmentRequestSent.cshtml.cs
    // Web Forms class retained as reference stub only — not active in ASP.NET Core pipeline.
    public partial class AppointmentNotificationSent
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Replaced by AppointmentRequestSentModel.OnGet() in AppointmentRequestSent.cshtml.cs
        }

        protected void sendARequest(object sender, EventArgs e)
        {
            // Replaced by AppointmentRequestSentModel.OnPostSendRequest() in AppointmentRequestSent.cshtml.cs
            // cr-dotnet-0045: Session["dID"]        -> RedisSessionHelper.GetString(_cache, "dID")
            // cr-dotnet-0045: Session["idoriginal"] -> RedisSessionHelper.GetInt32(_cache, "idoriginal")
            // cr-dotnet-0045: Session["freeSlot"]   -> RedisSessionHelper.GetString(_cache, "freeSlot")
        }
    }
}
