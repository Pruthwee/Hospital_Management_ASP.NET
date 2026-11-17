using System;
using System.Data;
using DBProject.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DBProject
{
    public class AppointmentNotificationSentModel : PageModel
    {
        public string Message = "";

        public void OnGet()
        {

        }


        //-----------------------Function1--------------------------//
        public IActionResult OnPostSendARequest()
        {
            myDAL objmyDAl = new myDAL();


            string dID1 = HttpContext.Session.GetString("dID");

            int dID = Convert.ToInt32(dID1);


            int pID = HttpContext.Session.GetRequiredInt32("idoriginal");


            string temp = HttpContext.Session.GetString("freeSlot");

            int freeSlot = Convert.ToInt32(temp);

            string mes = "";

            int status = objmyDAl.insertAppointment(dID, pID, freeSlot, ref mes);


            if (status == -1)
            {
                Message = "There was some error in sending appointment request to the Doctor.";
            }

            else
            {
                Message = mes;
            }

            return Page();
        }
    }
}
