using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using DBProject.DAL;

namespace DBProject.Patient
{
    /// <summary>
    /// Razor Page model for TreatmentHistory — replaces ASP.NET Web Forms TreatmentHistory.aspx / TreatmentHistory.aspx.cs.
    /// Eliminates ViewState, postbacks, and server affinity; enables stateless horizontal scaling on Azure Container Apps.
    ///
    /// Rule: cr-dotnet-1034 — Synchronous Data Binding in GridView Controls
    /// Remediation: Async GridView Binding with Azure SQL and Entity Framework Core
    ///   Synchronous THistoryGrid.DataSource = DT; THistoryGrid.DataBind() replaced with async/await pattern:
    ///   OnGetAsync() awaits getTreatmentHistoryAsync() from the DAL, preventing thread pool exhaustion
    ///   and enabling efficient auto-scaling in Azure cloud deployments.
    ///
    /// Rule: cr-dotnet-0045 — Session State Provider
    /// Remediation: Replaced InProc HttpSessionState with Azure Cache for Redis via IDistributedCache.
    /// Session value (idoriginal) is stored in and retrieved from Redis using RedisSessionHelper async methods,
    /// enabling distributed session state that scales across multiple container instances without server affinity.
    ///
    /// Migration: System.Web.UI.Page -> Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    /// Migration: System.Web.UI / System.Web.UI.WebControls -> ASP.NET Core Razor Pages model binding
    /// Migration: asp:Label (THistory) -> HistoryMessage model property rendered in .cshtml view
    /// Migration: asp:GridView (THistoryGrid) -> HTML table rendered from DataTable in Razor view
    /// Migration: Session["idoriginal"] (InProc) -> await RedisSessionHelper.GetInt32Async(_cache, "idoriginal") (Azure Cache for Redis)
    /// Migration: IsPostBack check -> OnGetAsync() in Razor Pages
    /// </summary>
    public class TreatmentHistoryModel : PageModel
    {
        private readonly IDistributedCache _cache;

        public TreatmentHistoryModel(IDistributedCache cache)
        {
            _cache = cache;
        }

        public string HistoryMessage { get; set; } = string.Empty;
        public DataTable? TreatmentHistory { get; set; }

        /// <summary>
        /// cr-dotnet-1034: Replaced synchronous Page_Load + THistoryGrid.DataBind() with async OnGetAsync().
        /// Awaits Redis cache operation and async DAL data retrieval to prevent thread pool exhaustion.
        /// </summary>
        public async Task OnGetAsync()
        {
            await LoadTreatmentHistoryAsync();
        }

        /// <summary>
        /// cr-dotnet-1034: Async data loading replaces synchronous getTreatmentHistory() + THistoryGrid.DataBind().
        /// Uses async DAL method and async Redis cache read to prevent blocking request threads under load.
        /// </summary>
        private async Task LoadTreatmentHistoryAsync()
        {
            var objmyDAl = new myDAL();

            // cr-dotnet-0045 + cr-dotnet-1034: Replaced HttpContext.Session.GetInt32 (InProc) with
            // async Redis-backed IDistributedCache to prevent thread pool exhaustion
            int id = await RedisSessionHelper.GetInt32Async(_cache, "idoriginal") ?? 0;

            // cr-dotnet-1034: Replaced synchronous getTreatmentHistory(id, ref DT) + THistoryGrid.DataBind()
            // with async getTreatmentHistoryAsync() to prevent thread pool exhaustion in Azure cloud deployments
            var (status, DT) = await objmyDAl.getTreatmentHistoryAsync(id);

            if (status == -1)
            {
                HistoryMessage = "There was some error in retrieving the Patient's Treatment History.";
            }
            else if (status == 0)
            {
                HistoryMessage = "There is currently no treatment history of yours.";
            }
            else
            {
                HistoryMessage = "Treatment History of " + status + " Appointment(s) is found: ";
                TreatmentHistory = DT;
            }
        }
    }
}
