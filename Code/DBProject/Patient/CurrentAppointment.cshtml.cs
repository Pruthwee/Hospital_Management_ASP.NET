using System;
using System.Data;
using DBProject.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DBProject
{
    public class CurrentAppointmentModel : PageModel
    {
        public string Appointment { get; set; }
        public string ADoctor { get; set; }
        public string ATimings { get; set; }

        public void OnGet()
        {
            appointmentToday();
        }

        //-----------------------Function1--------------------------//

        protected void appointmentToday()
        {
            myDAL objmyDAl = new myDAL();

            int pid = HttpContext.Session.GetRequiredInt32("idoriginal");

            string dName = "";
            string timings = "";

            int status = objmyDAl.appointmentTodayDisplayer(pid, ref dName, ref timings);

            if (status == -1)
            {
                Appointment = "There was some error in retrieving the Patient's appointment.";
            }

            else if (status == 0)
            {
                Appointment = "You have no appointment today with any doctor.";
            }

            else
            {
                if (status == 3)
                {
                    ADoctor = "You had an outdated appointment with Doctor " + dName + " to which he didn't respond. So that appointment is discarded.";
                    ATimings = "The Appointment Timings were : " + timings;
                    return;
                }

                else if (status == 2)
                {
                    ADoctor = "You have sent an appointment request to Doctor " + dName + " which isn't approved by him yet.";
                }

                else
                {
                    ADoctor = "Today you have an appointment with Doctor " + dName;
                }

                ATimings = "The Appointment Timings are : " + timings;
            }

            return;
        }
    }
}
