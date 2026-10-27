using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using System.Data;
using DBProject.DAL;

namespace DBProject.Doctor
{
    /// <summary>
    /// Razor Page model for DoctorHome — replaces ASP.NET Web Forms DoctorHome.aspx / DoctorHome.aspx.cs.
    /// Eliminates ViewState, postbacks, and server affinity; enables stateless horizontal scaling on Azure Container Apps.
    ///
    /// Rule: cr-dotnet-0045 — Session State Provider
    /// Remediation: Replaced InProc HttpSessionState with Azure Cache for Redis via IDistributedCache.
    /// Session value (idoriginal) is stored in and retrieved from Redis using RedisSessionHelper,
    /// enabling distributed session state that scales across multiple container instances without server affinity.
    /// </summary>
    public class DoctorHomeModel : PageModel
    {
        private readonly IDistributedCache _cache;

        public DoctorHomeModel(IDistributedCache cache)
        {
            _cache = cache;
        }

        public string DoctorName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string BirthDate { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string DeptNo { get; set; } = string.Empty;
        public string ChargesPerVisit { get; set; } = string.Empty;
        public string MonthlySalary { get; set; } = string.Empty;
        public string ReputeIndex { get; set; } = string.Empty;
        public string PatientsTreated { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string WorkExperience { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            var objmyDAL = new myDAL();
            DataTable dt = new DataTable();

            // cr-dotnet-0045: Replaced HttpContext.Session.GetInt32 (InProc) with Redis-backed IDistributedCache
            int did = RedisSessionHelper.GetInt32(_cache, "idoriginal") ?? 0;
            int found = objmyDAL.docinfo_DAL(did, ref dt);

            if (found != 1)
            {
                TempData["Error"] = "There was some error";
            }
            else if (dt.Rows.Count > 0)
            {
                // Map DataTable columns to model properties (preserving original column order)
                DoctorName      = dt.Rows[0][1].ToString() ?? string.Empty;
                Phone           = dt.Rows[0][2].ToString() ?? string.Empty;
                Address         = dt.Rows[0][3].ToString() ?? string.Empty;
                BirthDate       = dt.Rows[0][4].ToString() ?? string.Empty;
                Gender          = dt.Rows[0][5].ToString() ?? string.Empty;
                DeptNo          = dt.Rows[0][6].ToString() ?? string.Empty;
                ChargesPerVisit = dt.Rows[0][7].ToString() ?? string.Empty;
                MonthlySalary   = dt.Rows[0][8].ToString() ?? string.Empty;
                ReputeIndex     = dt.Rows[0][9].ToString() ?? string.Empty;
                PatientsTreated = dt.Rows[0][10].ToString() ?? string.Empty;
                Qualification   = dt.Rows[0][11].ToString() ?? string.Empty;
                Specialization  = dt.Rows[0][12].ToString() ?? string.Empty;
                WorkExperience  = dt.Rows[0][13].ToString() ?? string.Empty;
                Status          = dt.Rows[0][14].ToString() ?? string.Empty;
            }

            return Page();
        }
    }
}
