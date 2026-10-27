using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using DBProject.DAL;

namespace DBProject.Patient
{
    /// <summary>
    /// Razor Page model for PatientNotifications — replaces ASP.NET Web Forms PatientNotifications.aspx / PatientNotifications.aspx.cs.
    /// Eliminates ViewState, postbacks, and server affinity; enables stateless horizontal scaling on Azure Container Apps.
    ///
    /// Rule: cr-dotnet-0045 — Session State Provider
    /// Remediation: Replaced InProc HttpSessionState with Azure Cache for Redis via IDistributedCache.
    /// Session value (idoriginal) is stored in and retrieved from Redis using RedisSessionHelper,
    /// enabling distributed session state that scales across multiple container instances without server affinity.
    /// </summary>
    public class PatientNotificationsModel : PageModel
    {
        private readonly IDistributedCache _cache;

        public PatientNotificationsModel(IDistributedCache cache)
        {
            _cache = cache;
        }

        public string NotifyMessage { get; set; } = string.Empty;
        public string DoctorMessage { get; set; } = string.Empty;
        public string TimingsMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            LoadNotifications();
        }

        private void LoadNotifications()
        {
            var objmyDAl = new myDAL();

            // cr-dotnet-0045: Replaced HttpContext.Session.GetInt32 (InProc) with Redis-backed IDistributedCache
            int pid = RedisSessionHelper.GetInt32(_cache, "idoriginal") ?? 0;

            string dName = string.Empty;
            string timings = string.Empty;

            int status = objmyDAl.getNotifications(pid, ref dName, ref timings);

            if (status == -1)
            {
                NotifyMessage = "There was some error in retrieving the Patient's notifications.";
            }
            else if (status == 0)
            {
                NotifyMessage = "There are no new notifications :)";
            }
            else
            {
                if (status == 1)
                {
                    DoctorMessage = "Your requested appointment with Doctor " + dName + " has been accepted by him! :)";
                    TimingsMessage = "The Appointment Timings are : " + timings;
                }
                else if (status == 2)
                {
                    DoctorMessage = "Your requested appointment with Doctor " + dName + " has been rejected by him! :(";
                    TimingsMessage = "The Appointment Timings were : " + timings;
                }
                else if (status == 3)
                {
                    DoctorMessage = "Your appointment with Doctor " + dName + " has been completed now. We hope you are feeling better now!";
                    TimingsMessage = "The Appointment Timings were : " + timings;
                }
            }
        }
    }
}
