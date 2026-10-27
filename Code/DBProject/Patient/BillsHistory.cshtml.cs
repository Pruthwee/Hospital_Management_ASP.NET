using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using System.Data;
using System.Threading.Tasks;
using DBProject.DAL;

namespace DBProject.Patient
{
    /// <summary>
    /// Razor Page model for BillsHistory — replaces ASP.NET Web Forms BillsHistory.aspx / BillsHistory.aspx.cs.
    /// Eliminates ViewState, postbacks, and server affinity; enables stateless horizontal scaling on Azure Container Apps.
    ///
    /// Rule: cr-dotnet-0045 — Session State Provider
    /// Remediation: Replaced InProc HttpSessionState with Azure Cache for Redis via IDistributedCache.
    /// Session value (idoriginal) is stored in and retrieved from Redis using RedisSessionHelper,
    /// enabling distributed session state that scales across multiple container instances without server affinity.
    ///
    /// Rule: cr-dotnet-1034 — Synchronous Data Binding in GridView Controls
    /// Remediation: Async GridView Binding with Azure SQL and Entity Framework Core
    ///   OnGet() replaced with OnGetAsync() using async DAL methods (getBillHistoryAsync) and
    ///   async Redis cache operations (GetInt32Async) to prevent thread pool exhaustion and enable
    ///   efficient auto-scaling in Azure cloud deployments.
    ///   Connection strings are stored in Azure Key Vault and resolved via environment variables.
    /// </summary>
    public class BillsHistoryModel : PageModel
    {
        private readonly IDistributedCache _cache;

        public BillsHistoryModel(IDistributedCache cache)
        {
            _cache = cache;
        }

        public DataTable? BillHistory { get; set; }
        public string StatusMessage { get; set; } = string.Empty;

        // cr-dotnet-1034: Replaced synchronous OnGet() with async OnGetAsync() to prevent
        // thread pool exhaustion under load in Azure cloud deployments.
        public async Task OnGetAsync()
        {
            await LoadBillHistoryAsync();
        }

        private async Task LoadBillHistoryAsync()
        {
            var objmyDAl = new myDAL();

            // cr-dotnet-1034 + cr-dotnet-0045: Async Redis cache read prevents thread pool exhaustion
            int id = await RedisSessionHelper.GetInt32Async(_cache, "idoriginal") ?? 0;

            // cr-dotnet-1034: Replaced synchronous getBillHistory() with async getBillHistoryAsync()
            var (status, dt) = await objmyDAl.getBillHistoryAsync(id);

            if (status == -1)
            {
                StatusMessage = "There was some error in retrieving the Patient's Bill History.";
            }
            else if (status == 0)
            {
                StatusMessage = "There is currently no bill history of yours.";
            }
            else
            {
                StatusMessage = status + " Bill(s) are found: ";
                BillHistory = dt;
            }
        }
    }
}
