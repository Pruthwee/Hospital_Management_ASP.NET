using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using DBProject.DAL;

namespace DBProject.Doctor
{
    /// <summary>
    /// Razor Page model for HistoryUpdate — replaces ASP.NET Web Forms HistoryUpdate.aspx / HistoryUpdate.aspx.cs.
    /// Eliminates ViewState, postbacks, and server affinity; enables stateless horizontal scaling on Azure Container Apps.
    ///
    /// Rule: cr-dotnet-0045 — Session State Provider
    /// Remediation: Replaced InProc HttpSessionState with Azure Cache for Redis via IDistributedCache.
    /// Session values (idoriginal, appointid) are stored in and retrieved from Redis using RedisSessionHelper,
    /// enabling distributed session state that scales across multiple container instances without server affinity.
    /// </summary>
    public class HistoryUpdateModel : PageModel
    {
        private readonly IDistributedCache _cache;

        public HistoryUpdateModel(IDistributedCache cache)
        {
            _cache = cache;
        }

        [BindProperty]
        public string Disease { get; set; } = string.Empty;

        [BindProperty]
        public string Progress { get; set; } = string.Empty;

        [BindProperty]
        public string Prescription { get; set; } = string.Empty;

        public void OnGet()
        {
            // Page loads with empty form fields ready for doctor input
        }

        public IActionResult OnPostSaveInDatabase()
        {
            var objmyDAL = new myDAL();

            // cr-dotnet-0045: Replaced HttpContext.Session.GetInt32 (InProc) with Redis-backed IDistributedCache
            int did   = RedisSessionHelper.GetInt32(_cache, "idoriginal") ?? 0;
            int appid = RedisSessionHelper.GetInt32(_cache, "appointid") ?? 0;

            int found = objmyDAL.update_prescription_DAL(did, appid, Disease, Progress, Prescription);

            if (found != 1)
            {
                TempData["Error"] = "There was some error";
            }
            else
            {
                TempData["Success"] = "Information Successfully Updated";
            }

            return RedirectToPage();
        }

        public IActionResult OnPostGenerateBill()
        {
            return RedirectToPage("Bill");
        }
    }
}
