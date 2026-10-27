using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using DBProject.DAL;

namespace DBProject.Patient
{
    /// <summary>
    /// Razor Page model for CurrentAppointment — replaces ASP.NET Web Forms CurrentAppointment.aspx / CurrentAppointment.aspx.cs.
    /// Eliminates ViewState, postbacks, and server affinity; enables stateless horizontal scaling on Azure Container Apps.
    ///
    /// Rule: cr-dotnet-0045 — Session State Provider
    /// Remediation: Replaced InProc HttpSessionState with Azure Cache for Redis via IDistributedCache.
    /// Session value (idoriginal) is stored in and retrieved from Redis using RedisSessionHelper,
    /// enabling distributed session state that scales across multiple container instances without server affinity.
    /// </summary>
    public class CurrentAppointmentModel : PageModel
    {
        private readonly IDistributedCache _cache;

        public CurrentAppointmentModel(IDistributedCache cache)
        {
            _cache = cache;
        }

        public string AppointmentStatus { get; set; } = string.Empty;
        public string DoctorInfo { get; set; } = string.Empty;
        public string TimingInfo { get; set; } = string.Empty;

        public void OnGet()
        {
            LoadAppointmentToday();
        }

        private void LoadAppointmentToday()
        {
            var objmyDAl = new myDAL();

            // cr-dotnet-0045: Replaced HttpContext.Session.GetInt32 (InProc) with Redis-backed IDistributedCache
            int pid = RedisSessionHelper.GetInt32(_cache, "idoriginal") ?? 0;

            string dName = string.Empty;
            string timings = string.Empty;

            int status = objmyDAl.appointmentTodayDisplayer(pid, ref dName, ref timings);

            if (status == -1)
            {
                AppointmentStatus = "There was some error in retrieving the Patient's appointment.";
            }
            else if (status == 0)
            {
                AppointmentStatus = "You have no appointment today with any doctor.";
            }
            else
            {
                if (status == 3)
                {
                    DoctorInfo = "You had an outdated appointment with Doctor " + dName + " to which he didn't respond. So that appointment is discarded.";
                    TimingInfo = "The Appointment Timings were : " + timings;
                    return;
                }
                else if (status == 2)
                {
                    DoctorInfo = "You have sent an appointment request to Doctor " + dName + " which isn't approved by him yet.";
                }
                else
                {
                    DoctorInfo = "Today you have an appointment with Doctor " + dName;
                }

                TimingInfo = "The Appointment Timings are : " + timings;
            }
        }
    }
}
