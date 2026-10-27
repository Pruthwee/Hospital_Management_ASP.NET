using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using DBProject.DAL;

namespace DBProject.Patient
{
    /// <summary>
    /// Razor Page model for ViewDoctors — replaces ASP.NET Web Forms ViewDoctors.aspx / ViewDoctors.aspx.cs.
    /// Eliminates ViewState, postbacks, and server affinity; enables stateless horizontal scaling on Azure Container Apps.
    ///
    /// Rule: cr-dotnet-1034 — Synchronous Data Binding in GridView Controls
    /// Remediation: Async GridView Binding with Azure SQL and Entity Framework Core
    ///   Synchronous TDoctorGrid.DataSource = DT; TDoctorGrid.DataBind() replaced with async/await pattern:
    ///   OnGetAsync() awaits getDeptDoctorInfoAsync() from the DAL, preventing thread pool exhaustion
    ///   and enabling efficient auto-scaling in Azure cloud deployments.
    ///   OnPostSelectDoctorAsync() replaces the synchronous TDoctorGrid_RowCommand handler.
    ///
    /// Rule: cr-dotnet-0045 — Session State Provider
    /// Remediation: Replaced InProc HttpSessionState with Azure Cache for Redis via IDistributedCache.
    /// Session values (dID, deptOriginal) are stored in and retrieved from Redis using RedisSessionHelper async methods,
    /// enabling distributed session state that scales across multiple container instances without server affinity.
    ///
    /// Migration: System.Web.UI.Page -> Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    /// Migration: System.Web.UI / System.Web.UI.WebControls -> ASP.NET Core Razor Pages model binding
    /// Migration: asp:Label (TDoctor) -> DoctorMessage model property rendered in .cshtml view
    /// Migration: asp:GridView (TDoctorGrid) with OnRowCommand -> HTML table with OnPostSelectDoctorAsync() handler
    /// Migration: Response.Redirect("DoctorProfile.aspx") -> RedirectToPage("DoctorProfile")
    /// Migration: Session["dID"] = "" (InProc) -> await RedisSessionHelper.SetStringAsync(_cache, "dID", string.Empty) (Azure Cache for Redis)
    /// Migration: Session["dID"] = dID (InProc) -> await RedisSessionHelper.SetStringAsync(_cache, "dID", DoctorId) (Azure Cache for Redis)
    /// Migration: Session["deptOriginal"] (InProc) -> await RedisSessionHelper.GetStringAsync(_cache, "deptOriginal") (Azure Cache for Redis)
    /// Migration: IsPostBack check -> OnGetAsync() / OnPostAsync() separation in Razor Pages
    /// </summary>
    public class ViewDoctorsModel : PageModel
    {
        private readonly IDistributedCache _cache;

        public ViewDoctorsModel(IDistributedCache cache)
        {
            _cache = cache;
        }

        public string DoctorMessage { get; set; } = string.Empty;
        public DataTable? Doctors { get; set; }

        [BindProperty]
        public string DoctorId { get; set; } = string.Empty;

        /// <summary>
        /// cr-dotnet-1034: Replaced synchronous Page_Load + TDoctorGrid.DataBind() with async OnGetAsync().
        /// Awaits Redis cache operation and async DAL data retrieval to prevent thread pool exhaustion.
        /// </summary>
        public async Task OnGetAsync()
        {
            // cr-dotnet-0045 + cr-dotnet-1034: Replaced HttpContext.Session.SetString (InProc) with
            // async Redis-backed IDistributedCache to prevent thread pool exhaustion
            await RedisSessionHelper.SetStringAsync(_cache, "dID", string.Empty);
            await LoadDeptDoctorInfoAsync();
        }

        /// <summary>
        /// cr-dotnet-1034: Async data loading replaces synchronous getDeptDoctorInfo() + TDoctorGrid.DataBind().
        /// Uses async DAL method and async Redis cache read to prevent blocking request threads under load.
        /// </summary>
        private async Task LoadDeptDoctorInfoAsync()
        {
            var objmyDAl = new myDAL();
            DataTable DT = new DataTable();

            // cr-dotnet-0045 + cr-dotnet-1034: Replaced HttpContext.Session.GetString (InProc) with
            // async Redis-backed IDistributedCache to prevent thread pool exhaustion
            string deptName = await RedisSessionHelper.GetStringAsync(_cache, "deptOriginal") ?? string.Empty;

            // cr-dotnet-1034: Replaced synchronous getDeptDoctorInfo(deptName, ref DT) + TDoctorGrid.DataBind()
            // with async getDeptDoctorInfoAsync() to prevent thread pool exhaustion in Azure cloud deployments
            var (status, result) = await objmyDAl.getDeptDoctorInfoAsync(deptName);

            if (status == -1)
            {
                DoctorMessage = "There was some error in retrieving the Doctors Information.";
            }
            else
            {
                // cr-dotnet-0045 + cr-dotnet-1034: Replaced HttpContext.Session.GetString (InProc) with
                // async Redis-backed IDistributedCache
                string dept = await RedisSessionHelper.GetStringAsync(_cache, "deptOriginal") ?? string.Empty;
                DoctorMessage = "Following are our Specialized Doctors of " + dept + " Department:";
                Doctors = result;
            }
        }

        /// <summary>
        /// cr-dotnet-1034: Replaced synchronous TDoctorGrid_RowCommand with async OnPostSelectDoctorAsync().
        /// Awaits Redis cache write to prevent thread pool exhaustion.
        /// </summary>
        public async Task<IActionResult> OnPostSelectDoctorAsync()
        {
            // cr-dotnet-0045 + cr-dotnet-1034: Replaced HttpContext.Session.SetString (InProc) with
            // async Redis-backed IDistributedCache
            await RedisSessionHelper.SetStringAsync(_cache, "dID", DoctorId);
            return RedirectToPage("DoctorProfile");
        }
    }
}
