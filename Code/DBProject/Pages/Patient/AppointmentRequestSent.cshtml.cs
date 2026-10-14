using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject
{
    public partial class AppointmentNotificationSentModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppointmentNotificationSentModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string MessageText { get; set; } = "";

        public void OnGet()
        {
            sendARequest();
        }

        protected void sendARequest()
        {
            myDAL objmyDAl = new myDAL();

            string dID1 = _httpContextAccessor.HttpContext.Session.GetString("dID");
            int dID = Convert.ToInt32(dID1);

            int pID = (int)_httpContextAccessor.HttpContext.Session.GetInt32("idoriginal");

            string temp = _httpContextAccessor.HttpContext.Session.GetString("freeSlot");
            int freeSlot = Convert.ToInt32(temp);

            string mes = "";

            int status = objmyDAl.insertAppointment(dID, pID, freeSlot, ref mes);

            if (status == -1)
            {
                MessageText = "There was some error in sending appointment request to the Doctor.";
            }
            else
            {
                MessageText = mes;
            }
        }
    }
}
