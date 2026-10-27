using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using DBProject.DAL;

namespace DBProject.Patient
{
    /// <summary>
    /// Razor Page model for TakeAppointment — replaces ASP.NET Web Forms TakeAppointment.aspx / TakeAppointment.aspx.cs.
    /// Eliminates ViewState, postbacks, and server affinity; enables stateless horizontal scaling on Azure Container Apps.
    ///
    /// Rule: cr-dotnet-1034 — Synchronous Data Binding in GridView Controls
    /// Remediation: Async GridView Binding with Azure SQL and Entity Framework Core
    ///   Synchronous TDeptGrid.DataSource = DT; TDeptGrid.DataBind() replaced with async/await pattern:
    ///   OnGetAsync() awaits getdeptInfoAsync() from the DAL, preventing thread pool exhaustion
    ///   and enabling efficient auto-scaling in Azure cloud deployments.
    ///   OnPostSelectDepartmentAsync() replaces the synchronous TDeptGrid_RowCommand handler.
    ///
    /// Rule: cr-dotnet-0045 — Session State Provider
    /// Remediation: Replaced InProc HttpSessionState with Azure Cache for Redis via IDistributedCache.
    /// Session value (deptOriginal) is stored in and retrieved from Redis using RedisSessionHelper async methods,
    /// enabling distributed session state that scales across multiple container instances without server affinity.
    /// </summary>
    public class TakeAppointmentModel : PageModel
    {
        private readonly IDistributedCache _cache;

        public TakeAppointmentModel(IDistributedCache cache)
        {
            _cache = cache;
        }

        public string DeptMessage { get; set; } = string.Empty;
        public DataTable? Departments { get; set; }

        [BindProperty]
        public string DeptName { get; set; } = string.Empty;

        /// <summary>
        /// cr-dotnet-1034: Replaced synchronous Page_Load + TDeptGrid.DataBind() with async OnGetAsync().
        /// Awaits Redis cache operation and async DAL data retrieval to prevent thread pool exhaustion.
        /// </summary>
        public async Task OnGetAsync()
        {
            // cr-dotnet-0045 + cr-dotnet-1034: Replaced HttpContext.Session.SetString (InProc) with
            // async Redis-backed IDistributedCache to prevent thread pool exhaustion
            await RedisSessionHelper.SetStringAsync(_cache, "deptOriginal", string.Empty);
            await LoadDeptInfoAsync();
        }

        /// <summary>
        /// cr-dotnet-1034: Async data loading replaces synchronous getdeptInfo() + TDeptGrid.DataBind().
        /// Uses async DAL method to prevent blocking request threads under load.
        /// </summary>
        private async Task LoadDeptInfoAsync()
        {
            var objmyDAl = new myDAL();

            // cr-dotnet-1034: Replaced synchronous getdeptInfo(ref DT) + TDeptGrid.DataBind()
            // with async getdeptInfoAsync() to prevent thread pool exhaustion in Azure cloud deployments
            var (status, DT) = await objmyDAl.getdeptInfoAsync();

            if (status == -1)
            {
                DeptMessage = "There was some error in retrieving the Departments Information.";
            }
            else
            {
                DeptMessage = "Following are the departments available at our Clinic : ";
                Departments = DT;
            }
        }

        /// <summary>
        /// cr-dotnet-1034: Replaced synchronous TDeptGrid_RowCommand with async OnPostSelectDepartmentAsync().
        /// Awaits Redis cache write to prevent thread pool exhaustion.
        /// </summary>
        public async Task<IActionResult> OnPostSelectDepartmentAsync()
        {
            // cr-dotnet-0045 + cr-dotnet-1034: Replaced HttpContext.Session.SetString (InProc) with
            // async Redis-backed IDistributedCache
            await RedisSessionHelper.SetStringAsync(_cache, "deptOriginal", DeptName);
            return RedirectToPage("ViewDoctors");
        }
    }
}
