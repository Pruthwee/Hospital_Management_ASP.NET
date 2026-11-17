using System;
using System.Data;
using DBProject;
using DBProject.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace doctor
{
    public class pendingappointmentModel : PageModel
    {
        // GridView data source
        public DataTable Pendingappointments { get; set; }

        public void OnGet()
        {
            loadgrid();
        }

        public void loadgrid()
        {
            myDAL objDAL = new myDAL();

            int did = HttpContext.Session.GetRequiredInt32("idoriginal");

            DataTable DT = new DataTable();

            objDAL.GetAllpendingappointments_DAL(did, ref DT);

            Pendingappointments = DT;
        }

        // old update_appointment (RowCommand with CommandName == "Select")
        public IActionResult OnPostSelect(int rowIndex)
        {
            loadgrid(); // old Page_Load

            Int16 num = Convert.ToInt16(rowIndex);

            string aId = Pendingappointments.Rows[num][0].ToString();

            //retrieve appointmentid  from that row (key-non editable)
            int appointmentid = Convert.ToInt32(aId);

            //=====updating the newly entered values in database====
            myDAL objmyDAL = new myDAL();
            int result = objmyDAL.UpdateAppointment_DAL(appointmentid);
            //reload the page======================================================
            loadgrid();
            return Page();
        }

        // old Delete_appointment (RowDeleting)
        public IActionResult OnPostDelete(int rowIndex)
        {
            loadgrid(); // old Page_Load

            //get appointmentid from that row
            int appointmentid = Convert.ToInt32(Pendingappointments.Rows[rowIndex][0].ToString());

            myDAL objDAL = new myDAL();

            if (objDAL.Deleteappointment_DAL(appointmentid) == 1)
            {
                loadgrid(); //reload the grid to show the modifications in table
            }
            return Page();
        }
    }
}
