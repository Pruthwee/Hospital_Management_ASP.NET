// NOTE: This file has been superseded by DoctorProfile.cshtml.cs (ASP.NET Core Razor Pages).
// The Web Forms code-behind below is retained for reference only.
// Migration: System.Web.UI.Page -> Microsoft.AspNetCore.Mvc.RazorPages.PageModel
// Migration: System.Web.UI / System.Web.UI.WebControls -> ASP.NET Core Razor Pages model binding
//
// cr-dotnet-0045: Session State Provider — Azure Cache for Redis
// Migration: System.Web.SessionState (Session["key"]) InProc -> IDistributedCache (Azure Cache for Redis)
//   Line 29: Session["dID"]          -> RedisSessionHelper.GetString(_cache, "dID")
//   Line 45: Session["deptOriginal"] -> RedisSessionHelper.GetString(_cache, "deptOriginal")
// IDistributedCache is injected via constructor DI and backed by Azure Cache for Redis
// (Microsoft.Extensions.Caching.StackExchangeRedis), enabling stateless horizontal scaling.
//
// Migration: Label (DName, DPhone, etc.) -> DoctorProfileModel properties rendered in DoctorProfile.cshtml
// Migration: asp:Button OnClick="RedirectToAppointmentTaker" -> anchor link to /Patient/AppointmentTaker
// Migration: Response.Write("<script>alert(...)") -> DoctorProfileModel.ErrorMessage property
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
    // Migrated to Razor Pages: see Patient/DoctorProfile.cshtml and Patient/DoctorProfile.cshtml.cs
    // Web Forms class retained as reference stub only — not active in ASP.NET Core pipeline.
    public partial class DoctorProfile
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Replaced by DoctorProfileModel.OnGet() in DoctorProfile.cshtml.cs
            // cr-dotnet-0045: Session["dID"]          -> RedisSessionHelper.GetString(_cache, "dID")
            // cr-dotnet-0045: Session["deptOriginal"] -> RedisSessionHelper.GetString(_cache, "deptOriginal")
        }

        protected void doctorInfo(object sender, EventArgs e)
        {
            // Replaced by DoctorProfileModel.LoadDoctorInfo() in DoctorProfile.cshtml.cs
        }

        protected void RedirectToAppointmentTaker(object sender, EventArgs e)
        {
            // Replaced by anchor link to /Patient/AppointmentTaker in DoctorProfile.cshtml
        }
    }
}
