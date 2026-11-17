using System;
using System.Data;
using DBProject.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DBProject
{
    public class PatientNotificationsModel : PageModel
    {
        public string Notify { get; set; }
        public string NDoctor { get; set; }
        public string NTimings { get; set; }

        public void OnGet()
        {
            Notifications();
        }

        //-----------------------Function1--------------------------//

        protected void Notifications()
        {
            myDAL objmyDAl = new myDAL();

            int pid = HttpContext.Session.GetRequiredInt32("idoriginal");

            string dName = "";
            string timings = "";

            int status = objmyDAl.getNotifications(pid, ref dName, ref timings);

            if (status == -1)
            {
                Notify = "There was some error in retrieving the Patient's notifications.";
            }

            else if (status == 0)
            {
                Notify = "There are no new notifications :)";
            }

            else
            {
                if (status == 1)
                {
                    NDoctor = "Your requested appointment with Doctor " + dName + " has been accepted by him! :)";
                    NTimings = "The Appointment Timings are : " + timings;
                    return;
                }

                else if (status == 2)
                {
                    NDoctor = "Your requested appointment with Doctor " + dName + " has been rejected by him! :(";
                    NTimings = "The Appointment Timings were : " + timings;
                    return;
                }

                else if (status == 3)
                {
                    NDoctor = "Your appointment with Doctor " + dName + " has been completed now. We hope you are feeling better now!";
                    NTimings = "The Appointment Timings were : " + timings;
                    return;
                }

                return;
            }
        }
    }
}
