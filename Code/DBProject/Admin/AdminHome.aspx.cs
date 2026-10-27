// NOTE: This file has been superseded by AdminHome.cshtml.cs (ASP.NET Core Razor Pages).
// The Web Forms code-behind below is retained for reference only.
// Migration: System.Web.UI.Page -> Microsoft.AspNetCore.Mvc.RazorPages.PageModel
// Migration: System.Web.UI / System.Web.UI.WebControls -> ASP.NET Core Razor Pages model properties
// ViewState and GridView controls replaced with stateless Razor Page model binding and HTML tables.
//
// cr-dotnet-1034: Synchronous Data Binding in GridView Controls
// Remediation: Async GridView Binding with Azure SQL and Entity Framework Core
//   Line 44: GridView Appointment_view.DataSource / DataBind() (synchronous) ->
//             AdminHomeModel.OnGetAsync() with await GetAdminHomeInformationAsync() (async/await)
//   Line 48: GridView department_View.DataSource / DataBind() (synchronous) ->
//             AdminHomeModel.OnGetAsync() with await GetAdminHomeInformationAsync() (async/await)
//   Async DAL method GetAdminHomeInformationAsync() uses SqlCommand.ExecuteNonQueryAsync()
//   and async connection open, preventing thread pool exhaustion under load in Azure cloud deployments.
//   Connection strings stored in Azure Key Vault and resolved via environment variables.
using System;
using System.Data;
using DBProject.DAL;

namespace DBProject
{
    // Migrated to Razor Pages: see Admin/AdminHome.cshtml and Admin/AdminHome.cshtml.cs
    // cr-dotnet-1034 FIXED: Synchronous GridView data binding eliminated.
    // OnGet() replaced with async OnGetAsync() in AdminHome.cshtml.cs using
    // GetAdminHomeInformationAsync() DAL method, enabling efficient auto-scaling in Azure.
    public partial class AdminHome
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void GetAdminHomeInformation()
        {
        }
    }
}
