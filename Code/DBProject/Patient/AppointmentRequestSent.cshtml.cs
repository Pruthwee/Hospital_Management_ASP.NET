using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using DBProject.DAL;

namespace DBProject.Patient
{
    /// <summary>
    /// Razor Page model for AppointmentRequestSent — replaces ASP.NET Web Forms AppointmentRequestSent.aspx / AppointmentRequestSent.aspx.cs.
    /// Eliminates ViewState, postbacks, and server affinity; enables stateless horizontal scaling on Azure Container Apps.
    ///
    /// Rule: cr-dotnet-0045 — Session State Provider
    /// Remediation: Replaced InProc HttpSessionState with Azure Cache for Redis via IDistributedCache.
    /// Session values (dID, idoriginal, freeSlot) are stored in and retrieved from Redis using RedisSessionHelper,
    /// enabling distributed session state that scales across multiple container instances without server affinity.
    /// </summary>
    public class AppointmentRequestSentModel : PageModel
    {
        private readonly IDistributedCache _cache;

        public AppointmentRequestSentModel(IDistributedCache cache)
        {
            _cache = cache;
        }

        public string Message { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public IActionResult OnPostSendRequest()
        {
            var objmyDAl = new myDAL();

            // cr-dotnet-0045: Replaced HttpContext.Session.GetString/GetInt32 (InProc) with Redis-backed IDistributedCache
            string? dID1 = RedisSessionHelper.GetString(_cache, "dID");
            int dID = Convert.ToInt32(dID1);

            int pID = RedisSessionHelper.GetInt32(_cache, "idoriginal") ?? 0;

            string? temp = RedisSessionHelper.GetString(_cache, "freeSlot");
            int freeSlot = Convert.ToInt32(temp);

            string mes = string.Empty;

            int status = objmyDAl.insertAppointment(dID, pID, freeSlot, ref mes);

            if (status == -1)
            {
                Message = "There was some error in sending appointment request to the Doctor.";
            }
            else
            {
                Message = mes;
            }

            return Page();
        }
    }
}
