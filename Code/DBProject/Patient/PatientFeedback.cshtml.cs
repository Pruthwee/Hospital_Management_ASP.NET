using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using DBProject.DAL;

namespace DBProject.Patient
{
    /// <summary>
    /// Razor Page model for PatientFeedback — replaces ASP.NET Web Forms PatientFeedback.aspx / PatientFeedback.aspx.cs.
    /// Eliminates ViewState, postbacks, and server affinity; enables stateless horizontal scaling on Azure Container Apps.
    ///
    /// Rule: cr-dotnet-0045 — Session State Provider
    /// Remediation: Replaced InProc HttpSessionState with Azure Cache for Redis via IDistributedCache.
    /// Session values (idoriginal, aID) are stored in and retrieved from Redis using RedisSessionHelper,
    /// enabling distributed session state that scales across multiple container instances without server affinity.
    /// </summary>
    public class PatientFeedbackModel : PageModel
    {
        private readonly IDistributedCache _cache;

        public PatientFeedbackModel(IDistributedCache cache)
        {
            _cache = cache;
        }

        public string FeedbackStatus { get; set; } = string.Empty;
        public string DoctorInfo { get; set; } = string.Empty;
        public string TimingInfo { get; set; } = string.Empty;
        public string FeedbackResult { get; set; } = string.Empty;
        public bool ShowFeedbackForm { get; set; } = false;

        [BindProperty]
        public int Rating { get; set; } = 1;

        public void OnGet()
        {
            // cr-dotnet-0045: Replaced HttpContext.Session.SetString (InProc) with Redis-backed IDistributedCache
            RedisSessionHelper.SetString(_cache, "aID", string.Empty);
            LoadPendingFeedback();
        }

        private void LoadPendingFeedback()
        {
            var objmyDAl = new myDAL();

            // cr-dotnet-0045: Replaced HttpContext.Session.GetInt32 (InProc) with Redis-backed IDistributedCache
            int pid = RedisSessionHelper.GetInt32(_cache, "idoriginal") ?? 0;

            string dName = string.Empty;
            string timings = string.Empty;
            int aID = 0;

            int status = objmyDAl.isFeedbackPending(pid, ref dName, ref timings, ref aID);

            if (status == -1)
            {
                FeedbackStatus = "There was some error in retrieving the Pending Feedbacks.";
            }
            else if (status == 0)
            {
                FeedbackStatus = "There are no pending feedbacks :)";
            }
            else
            {
                // cr-dotnet-0045: Replaced HttpContext.Session.SetInt32 (InProc) with Redis-backed IDistributedCache
                RedisSessionHelper.SetInt32(_cache, "aID", aID);

                DoctorInfo = "Your feedback for the appointment with Doctor " + dName + " is pending. Kindly give it.";
                TimingInfo = "The Appointment Timings were : " + timings;
                ShowFeedbackForm = true;
            }
        }

        public IActionResult OnPostGiveFeedback()
        {
            var objmyDAl = new myDAL();

            // cr-dotnet-0045: Replaced HttpContext.Session.GetInt32 (InProc) with Redis-backed IDistributedCache
            int aID = RedisSessionHelper.GetInt32(_cache, "aID") ?? 0;

            // Rating is bound from the form but not used in the current DAL method signature
            int status = objmyDAl.givePendingFeedback(aID);

            if (status == -1)
            {
                FeedbackResult = "There was some error.";
            }
            else if (status == 0)
            {
                FeedbackResult = "Thank you for your feedback :)";
            }

            // Re-load pending feedback state for display
            LoadPendingFeedback();

            return Page();
        }
    }
}
