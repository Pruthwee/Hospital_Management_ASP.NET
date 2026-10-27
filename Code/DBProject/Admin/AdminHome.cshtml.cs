using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Threading.Tasks;
using DBProject.DAL;

namespace DBProject.Admin
{
    /// <summary>
    /// Razor Page model for Admin Home — replaces ASP.NET Web Forms AdminHome.aspx / AdminHome.aspx.cs.
    /// Eliminates ViewState, postbacks, and server affinity; enables stateless horizontal scaling on Azure Container Apps.
    ///
    /// Rule: cr-dotnet-1034 — Synchronous Data Binding in GridView Controls
    /// Remediation: Async GridView Binding with Azure SQL and Entity Framework Core
    ///   OnGet() replaced with OnGetAsync() using async DAL methods (GetAdminHomeInformationAsync)
    ///   to prevent thread pool exhaustion and enable efficient auto-scaling in Azure cloud deployments.
    ///   Connection strings are stored in Azure Key Vault and resolved via environment variables.
    /// </summary>
    public class AdminHomeModel : PageModel
    {
        public string TotalDoctors { get; set; } = string.Empty;
        public string TotalPatients { get; set; } = string.Empty;
        public string TotalIncome { get; set; } = string.Empty;
        public DataTable? Appointments { get; set; }
        public DataTable? Departments { get; set; }

        // cr-dotnet-1034: Replaced synchronous OnGet() with async OnGetAsync() to prevent
        // thread pool exhaustion under load in Azure cloud deployments.
        public async Task OnGetAsync()
        {
            await GetAdminHomeInformationAsync();
        }

        private async Task GetAdminHomeInformationAsync()
        {
            var objmyDAL = new myDAL();
            DataTable[] arrTable = new DataTable[5];
            for (int i = 0; i < 5; i++)
            {
                arrTable[i] = new DataTable();
            }

            // cr-dotnet-1034: Replaced synchronous GetAdminHomeInformation() with async variant
            // using SqlCommand.ExecuteNonQueryAsync() and async connection open, preventing
            // thread pool exhaustion and enabling efficient auto-scaling in Azure cloud deployments.
            await objmyDAL.GetAdminHomeInformationAsync(arrTable);

            TotalDoctors = arrTable[0].Rows.Count > 0 ? arrTable[0].Rows[0][0].ToString() ?? string.Empty : string.Empty;
            TotalPatients = arrTable[1].Rows.Count > 0 ? arrTable[1].Rows[0][0].ToString() ?? string.Empty : string.Empty;
            TotalIncome = arrTable[2].Rows.Count > 0 ? arrTable[2].Rows[0][0].ToString() ?? string.Empty : string.Empty;
            Departments = arrTable[3];
            Appointments = arrTable[4];
        }
    }
}
