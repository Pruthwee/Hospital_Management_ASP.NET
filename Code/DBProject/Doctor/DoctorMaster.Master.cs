// NOTE: This file has been superseded by _DoctorLayout.cshtml (ASP.NET Core Razor Pages Layout).
// The Web Forms Master Page code-behind below is retained for reference only.
// Migration: System.Web.UI.MasterPage -> Razor Layout page (_DoctorLayout.cshtml)
// Migration: System.Web.UI / System.Web.UI.WebControls -> ASP.NET Core Razor layout with @RenderBody() / @RenderSection()
// Migration: <asp:ContentPlaceHolder> -> @RenderBody() / @RenderSection() in Razor layout
// ViewState and server-side form lifecycle eliminated; stateless horizontal scaling enabled for Azure Container Apps.
// using System.Web;                  // REMOVED: Not available in ASP.NET Core
// using System.Web.UI;               // REMOVED: Web Forms MasterPage base class not used in Razor Pages
// using System.Web.UI.WebControls;   // REMOVED: Server controls replaced with HTML + Tag Helpers
using System;
using System.Collections.Generic;
using System.Linq;

namespace doctor
{
    // Migrated to Razor Layout: see Doctor/_DoctorLayout.cshtml
    // Web Forms class retained as reference stub only — not active in ASP.NET Core pipeline.
    public partial class doctormaster
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Replaced by _DoctorLayout.cshtml Razor layout page
        }
    }
}
