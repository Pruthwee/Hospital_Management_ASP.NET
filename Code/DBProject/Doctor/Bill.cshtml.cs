using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using System.Data;
using DBProject.DAL;

namespace DBProject.Doctor
{
    /// <summary>
    /// Razor Page model for Bill — replaces ASP.NET Web Forms Bill.aspx / Bill.aspx.cs.
    /// Eliminates ViewState, postbacks, and server affinity; enables stateless horizontal scaling on Azure Container Apps.
    ///
    /// Rule: cr-dotnet-0045 — Session State Provider
    /// Remediation: Replaced InProc HttpSessionState with Azure Cache for Redis via IDistributedCache.
    /// Session values (idoriginal, appointid) are stored in and retrieved from Redis using RedisSessionHelper,
    /// enabling distributed session state that scales across multiple container instances without server affinity.
    /// </summary>
    public class BillModel : PageModel
    {
        private readonly IDistributedCache _cache;

        public BillModel(IDistributedCache cache)
        {
            _cache = cache;
        }

        public string BillAmount { get; set; } = string.Empty;

        public void OnGet()
        {
            var objmyDAL = new myDAL();
            DataTable dt = new DataTable();

            // cr-dotnet-0045: Replaced HttpContext.Session.GetInt32 (InProc) with Redis-backed IDistributedCache
            int did = RedisSessionHelper.GetInt32(_cache, "idoriginal") ?? 0;
            int found = objmyDAL.generate_bill_DAL(did, ref dt);

            if (found != 1)
            {
                TempData["Error"] = "There was some error";
            }
            else if (dt.Rows.Count > 0)
            {
                BillAmount = dt.Rows[0][0].ToString() ?? string.Empty;
            }
        }

        public IActionResult OnPostBillPaid()
        {
            var objmyDAL = new myDAL();
            // cr-dotnet-0045: Replaced HttpContext.Session.GetInt32 (InProc) with Redis-backed IDistributedCache
            int did = RedisSessionHelper.GetInt32(_cache, "idoriginal") ?? 0;
            int appoint = RedisSessionHelper.GetInt32(_cache, "appointid") ?? 0;
            objmyDAL.paid_bill_DAL(did, appoint);
            return RedirectToPage("PatientHistory");
        }

        public IActionResult OnPostBillUnpaid()
        {
            var objmyDAL = new myDAL();
            // cr-dotnet-0045: Replaced HttpContext.Session.GetInt32 (InProc) with Redis-backed IDistributedCache
            int did = RedisSessionHelper.GetInt32(_cache, "idoriginal") ?? 0;
            int appoint = RedisSessionHelper.GetInt32(_cache, "appointid") ?? 0;
            objmyDAL.Unpaid_bill_DAL(did, appoint);
            return RedirectToPage("PatientHistory");
        }
    }
}
