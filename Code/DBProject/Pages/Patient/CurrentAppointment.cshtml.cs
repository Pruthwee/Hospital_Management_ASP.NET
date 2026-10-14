using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject
{
    public partial class CurrentAppointmentModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentAppointmentModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string AppointmentText { get; set; } = "";
        public string ADoctorText { get; set; } = "";
        public string ATimingsText { get; set; } = "";

        public void OnGet()
        {
            appointmentToday();
        }

        protected void appointmentToday()
        {
            myDAL objmyDAl = new myDAL();

            int pid = (int)_httpContextAccessor.HttpContext.Session.GetInt32("idoriginal");

            string dName = "";
            string timings = "";

            int status = objmyDAl.appointmentTodayDisplayer(pid, ref dName, ref timings);

            if (status == -1)
            {
                AppointmentText = "There was some error in retrieving the Patient's appointment.";
            }
            else if (status == 0)
            {
                AppointmentText = "You have no appointment today with any doctor.";
            }
            else
            {
                if (status == 3)
                {
                    ADoctorText = "You had an outdated appointment with Doctor " + dName + " to which he didn't respond. So that appointment is discarded.";
                    ATimingsText = "The Appointment Timings were : " + timings;
                    return;
                }
                else if (status == 2)
                {
                    ADoctorText = "You have sent an appointment request to Doctor " + dName + " which isn't approved by him yet.";
                }
                else
                {
                    ADoctorText = "Today you have an appointment with Doctor " + dName;
                }

                ATimingsText = "The Appointment Timings are : " + timings;
            }
        }
    }
}
