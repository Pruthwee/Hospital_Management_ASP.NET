using System;
using System.Data;
using DBProject.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DBProject
{
    public class AppointmentTakerModel : PageModel
    {
        public string PAppointment = "";

        // Data of the grid (null when it was not bound, like the old unbound GridView)
        public DataTable PAppointmentGrid = null;

        // Page_Load ran on every request (also on postbacks)
        public void OnGet()
        {
            pageLoad();
        }

        private void pageLoad()
        {
            HttpContext.Session.SetString("freeSlot", "");
            freeSlots();
        }


        //---------------Function Called whenever a Free Slot is selected from the Grid View----//
        public IActionResult OnPostSelect(int rowIndex)
        {
            pageLoad();

            Int16 num = Convert.ToInt16(rowIndex);

            // Cells[2] of the old grid = first data column (Select button and No. come before it)
            string appointment = Convert.ToString(PAppointmentGrid.Rows[num][0]);

            string[] tokens = appointment.Split(':');

            HttpContext.Session.SetString("freeSlot", tokens[0]);

            return Redirect("AppointmentRequestSent.aspx");
        }


        //-----------------------Function1--------------------------//

        protected void freeSlots()
        {
            myDAL objmyDAl = new myDAL();

            DataTable DT = new DataTable();


            string dID1 = HttpContext.Session.GetString("dID");

            int dID = Convert.ToInt32(dID1);


            int pID = HttpContext.Session.GetRequiredInt32("idoriginal");


            int status = objmyDAl.getFreeSlots(dID, pID, ref DT);


            if (status == -1)
            {
                PAppointment = "There was some error in retrieving the Doctors's Free Slots.";
            }

            else if (status == 0)
            {
                PAppointment = "There is currently no free slot of this doctor.";
            }

            else if (status > 0)
            {
                PAppointment = "The following are the " + status + " free slots of this doctor for today :";
                PAppointmentGrid = DT;
            }

            return;
        }
    }
}
