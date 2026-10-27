using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using System.Data;
using System.Threading.Tasks;
using DBProject.DAL;

namespace DBProject.Doctor
{
    /// <summary>
    /// Razor Page model for PatientHistory — replaces ASP.NET Web Forms PatientHistory.aspx / PatientHistory.aspx.cs.
    /// Eliminates ViewState, postbacks, and server affinity; enables stateless horizontal scaling on Azure Container Apps.
    ///
    /// Rule: cr-dotnet-0045 — Session State Provider
    /// Remediation: Replaced InProc HttpSessionState with Azure Cache for Redis via IDistributedCache.
    /// Session values (idoriginal, appointid) are stored in and retrieved from Redis using RedisSessionHelper,
    /// enabling distributed session state that scales across multiple container instances without server affinity.
    ///
    /// Rule: cr-dotnet-1034 — Synchronous Data Binding in GridView Controls
    /// Remediation: Async GridView Binding with Azure SQL and Entity Framework Core
    ///   OnGet() replaced with OnGetAsync() using async DAL methods (search_patient_DALAsync) and
    ///   async Redis cache operations (GetInt32Async) to prevent thread pool exhaustion and enable
    ///   efficient auto-scaling in Azure cloud deployments.
    ///   Connection strings are stored in Azure Key Vault and resolved via environment variables.
    /// </summary>
    public class PatientHistoryModel : PageModel
    {
        private readonly IDistributedCache _cache;

        public PatientHistoryModel(IDistributedCache cache)
        {
            _cache = cache;
        }

        public DataTable? Patients { get; set; }

        [BindProperty]
        public int AppointmentId { get; set; }

        // cr-dotnet-1034: Replaced synchronous OnGet() with async OnGetAsync() to prevent
        // thread pool exhaustion under load in Azure cloud deployments.
        public async Task OnGetAsync()
        {
            var objmydal = new myDAL();

            // cr-dotnet-1034 + cr-dotnet-0045: Async Redis cache read prevents thread pool exhaustion
            int did = await RedisSessionHelper.GetInt32Async(_cache, "idoriginal") ?? 0;

            // cr-dotnet-1034: Replaced synchronous search_patient_DAL() with async search_patient_DALAsync()
            var (found, dt) = await objmydal.search_patient_DALAsync(did);

            if (found != 1)
            {
                TempData["Error"] = "There was some error";
            }
            else
            {
                Patients = dt;
            }
        }

        public async Task<IActionResult> OnPostSelectPatientAsync()
        {
            // cr-dotnet-1034 + cr-dotnet-0045: Async Redis cache write prevents thread pool exhaustion
            await RedisSessionHelper.SetInt32Async(_cache, "appointid", AppointmentId);
            return RedirectToPage("HistoryUpdate");
        }
    }
}
