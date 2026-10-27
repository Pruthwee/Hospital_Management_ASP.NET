// NOTE: This file has been superseded by _PatientLayout.cshtml (ASP.NET Core Razor Pages Layout).
// The Web Forms Master Page code-behind below is retained for reference only.
// Migration: System.Web.UI.MasterPage -> ASP.NET Core Razor Layout (_PatientLayout.cshtml)
// Migration: System.Web.UI / System.Web.UI.WebControls -> ASP.NET Core Razor Pages layout model
// Migration: <asp:ContentPlaceHolder> -> @RenderBody() / @RenderSection() in Razor Layout
// Migration: <form runat="server"> -> Standard HTML form elements in Razor Pages
// Migration: MasterPage server-side form -> Stateless Razor Layout without server affinity
// ViewState and postback patterns replaced with stateless Razor Layout.
// using System.Web;                  // REMOVED: Not available in ASP.NET Core
// using System.Web.UI;               // REMOVED: Web Forms MasterPage base class not used in Razor Pages
// using System.Web.UI.WebControls;   // REMOVED: Server controls replaced with HTML + Tag Helpers
using System;
using System.Collections.Generic;
using System.Linq;

namespace DBProject
{
    // Migrated to Razor Layout: see Patient/_PatientLayout.cshtml
    // Web Forms class retained as reference stub only — not active in ASP.NET Core pipeline.
    public partial class PatientMaster
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Replaced by _PatientLayout.cshtml in ASP.NET Core Razor Pages
        }
    }
}
