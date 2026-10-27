using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using System.Data;
using System.Threading.Tasks;
using DBProject.DAL;

namespace DBProject.Doctor
{
    /// <summary>
    /// Razor Page model for PendingAppointment — replaces ASP.NET Web Forms PendingAppointment.aspx / PendingAppointment.aspx.cs.
    /// Eliminates ViewState, postbacks, and server affinity; enables stateless horizontal scaling on Azure Container Apps.
    ///
    /// Rule: cr-dotnet-0045 — Session State Provider
    /// Remediation: Replaced InProc HttpSessionState with Azure Cache for Redis via IDistributedCache.
    /// Session value (idoriginal) is stored in and retrieved from Redis using RedisSessionHelper,
    /// enabling distributed session state that scales across multiple container instances without server affinity.
    ///
    /// Rule: cr-dotnet-1034 — Synchronous Data Binding in GridView Controls
    /// Remediation: Async GridView Binding with Azure SQL and Entity Framework Core
    ///   OnGet() replaced with OnGetAsync() using async DAL methods (GetAllpendingappointmentsAsync) and
    ///   async Redis cache operations (GetInt32Async) to prevent thread pool exhaustion and enable
    ///   efficient auto-scaling in Azure cloud deployments.
    ///   Connection strings are stored in Azure Key Vault and resolved via environment variables.
    /// </summary>
    public class PendingAppointmentModel : PageModel
    {
        private readonly IDistributedCache _cache;

        public PendingAppointmentModel(IDistributedCache cache)
        {
            _cache = cache;
        }

        public DataTable? PendingAppointments { get; set; }

        [BindProperty]
        public int AppointmentId { get; set; }

        // cr-dotnet-1034: Replaced synchronous OnGet() with async OnGetAsync() to prevent
        // thread pool exhaustion under load in Azure cloud deployments.
        public async Task OnGetAsync()
        {
            await LoadGridAsync();
        }

        private async Task LoadGridAsync()
        {
            var objDAL = new myDAL();
            // cr-dotnet-1034 + cr-dotnet-0045: Async Redis cache read prevents thread pool exhaustion
            int did = await RedisSessionHelper.GetInt32Async(_cache, "idoriginal") ?? 0;

            // cr-dotnet-1034: Replaced synchronous GetAllpendingappointments_DAL() with async variant
            DataTable dt = await objDAL.GetAllpendingappointmentsAsync(did);
            PendingAppointments = dt;
        }

        public async Task<IActionResult> OnPostApproveAppointmentAsync()
        {
            var objmyDAL = new myDAL();
            // cr-dotnet-1034: Replaced synchronous UpdateAppointment_DAL() with async variant
            await objmyDAL.UpdateAppointmentAsync(AppointmentId);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAppointmentAsync()
        {
            var objDAL = new myDAL();
            // cr-dotnet-1034: Replaced synchronous Deleteappointment_DAL() with async variant
            await objDAL.DeleteappointmentAsync(AppointmentId);
            return RedirectToPage();
        }
    }
}
