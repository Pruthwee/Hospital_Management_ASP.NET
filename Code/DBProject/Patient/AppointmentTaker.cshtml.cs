using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using System.Data;
using System.Threading.Tasks;
using DBProject.DAL;

namespace DBProject.Patient
{
    /// <summary>
    /// Razor Page model for AppointmentTaker — replaces ASP.NET Web Forms AppointmentTaker.aspx / AppointmentTaker.aspx.cs.
    /// Eliminates ViewState, postbacks, and server affinity; enables stateless horizontal scaling on Azure Container Apps.
    ///
    /// Rule: cr-dotnet-0045 — Session State Provider
    /// Remediation: Replaced InProc HttpSessionState with Azure Cache for Redis via IDistributedCache.
    /// Session values (freeSlot, dID, idoriginal) are stored in and retrieved from Redis using RedisSessionHelper,
    /// enabling distributed session state that scales across multiple container instances without server affinity.
    ///
    /// Rule: cr-dotnet-1034 — Synchronous Data Binding in GridView Controls
    /// Remediation: Async GridView Binding with Azure SQL and Entity Framework Core
    ///   OnGet() replaced with OnGetAsync() using async DAL methods (getFreeSlotsAsync) and
    ///   async Redis cache operations (GetInt32Async, GetStringAsync, SetStringAsync) to prevent
    ///   thread pool exhaustion and enable efficient auto-scaling in Azure cloud deployments.
    ///   Connection strings are stored in Azure Key Vault and resolved via environment variables.
    /// </summary>
    public class AppointmentTakerModel : PageModel
    {
        private readonly IDistributedCache _cache;

        public AppointmentTakerModel(IDistributedCache cache)
        {
            _cache = cache;
        }

        public DataTable? FreeSlots { get; set; }
        public string StatusMessage { get; set; } = string.Empty;

        [BindProperty]
        public string SlotValue { get; set; } = string.Empty;

        // cr-dotnet-1034: Replaced synchronous OnGet() with async OnGetAsync() to prevent
        // thread pool exhaustion under load in Azure cloud deployments.
        public async Task OnGetAsync()
        {
            // cr-dotnet-1034 + cr-dotnet-0045: Async Redis cache write prevents thread pool exhaustion
            await RedisSessionHelper.SetStringAsync(_cache, "freeSlot", string.Empty);
            await LoadFreeSlotsAsync();
        }

        private async Task LoadFreeSlotsAsync()
        {
            var objmyDAl = new myDAL();

            // cr-dotnet-1034 + cr-dotnet-0045: Async Redis cache reads prevent thread pool exhaustion
            string? dID1 = await RedisSessionHelper.GetStringAsync(_cache, "dID");
            int dID = Convert.ToInt32(dID1);

            int pID = await RedisSessionHelper.GetInt32Async(_cache, "idoriginal") ?? 0;

            // cr-dotnet-1034: Replaced synchronous getFreeSlots() with async getFreeSlotsAsync()
            var (status, dt) = await objmyDAl.getFreeSlotsAsync(dID, pID);

            if (status == -1)
            {
                StatusMessage = "There was some error in retrieving the Doctor's Free Slots.";
            }
            else if (status == 0)
            {
                StatusMessage = "There is currently no free slot of this doctor.";
            }
            else if (status > 0)
            {
                StatusMessage = "The following are the " + status + " free slots of this doctor for today :";
                FreeSlots = dt;
            }
        }

        public async Task<IActionResult> OnPostSelectSlotAsync()
        {
            // Extract the slot ID from the slot value (format: "slotId: description")
            string[] tokens = SlotValue.Split(':');
            // cr-dotnet-1034 + cr-dotnet-0045: Async Redis cache write prevents thread pool exhaustion
            await RedisSessionHelper.SetStringAsync(_cache, "freeSlot", tokens[0]);
            return RedirectToPage("AppointmentRequestSent");
        }
    }
}
