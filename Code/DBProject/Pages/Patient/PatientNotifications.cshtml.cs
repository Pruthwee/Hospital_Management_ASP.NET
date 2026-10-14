using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject
{
    public partial class PatientNotificationsModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PatientNotificationsModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string NotifyText { get; set; } = "";
        public string NDoctorText { get; set; } = "";
        public string NTimingsText { get; set; } = "";

        public void OnGet()
        {
            Notifications();
        }

        protected void Notifications()
        {
            myDAL objmyDAl = new myDAL();

            int pid = (int)_httpContextAccessor.HttpContext.Session.GetInt32("idoriginal");

            string dName = "";
            string timings = "";

            int status = objmyDAl.getNotifications(pid, ref dName, ref timings);

            if (status == -1)
            {
                NotifyText = "There was some error in retrieving the Patient's notifications.";
            }
            else if (status == 0)
            {
                NotifyText = "There are no new notifications :)";
            }
            else
            {
                if (status == 1)
                {
                    NDoctorText = "Your requested appointment with Doctor " + dName + " has been accepted by him! :)";
                    NTimingsText = "The Appointment Timings are : " + timings;
                    return;
                }
                else if (status == 2)
                {
                    NDoctorText = "Your requested appointment with Doctor " + dName + " has been rejected by him! :(";
                    NTimingsText = "The Appointment Timings were : " + timings;
                    return;
                }
                else if (status == 3)
                {
                    NDoctorText = "Your appointment with Doctor " + dName + " has been completed now. We hope you are feeling better now!";
                    NTimingsText = "The Appointment Timings were : " + timings;
                    return;
                }
                return;
            }
        }
    }
}
