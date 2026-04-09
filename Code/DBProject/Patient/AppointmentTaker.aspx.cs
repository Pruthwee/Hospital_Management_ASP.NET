using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DBProject.DAL;
using DBProject.Helpers;
using System.Data;


namespace DBProject
{
    public partial class AppointmentTaker : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check authentication using cloud-ready session helper
            if (!SessionHelper.IsAuthenticated)
            {
                Response.Redirect("~/SignUp.aspx");
                return;
            }

            // Use cloud-ready session helper
            SessionHelper.SetValue("freeSlot", "");
            freeSlots(sender, e);
        }

        //---------------Function Called whenever a Free Slot is selected from the Grid View----//
        protected void PAppointmentGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                Int16 num = Convert.ToInt16(e.CommandArgument);

                string appointment = PAppointmentGrid.Rows[num].Cells[2].Text;

                string[] tokens = appointment.Split(':');

                // Use cloud-ready session helper
                SessionHelper.SetValue("freeSlot", tokens[0]);

                Response.BufferOutput = true;
                Response.Redirect("AppointmentRequestSent.aspx");

                return;
            }
        }

        //-----------------------Function1--------------------------//
        protected void freeSlots(object sender, EventArgs e)
        {
            myDAL objmyDAl = new myDAL();

            DataTable DT = new DataTable();

            // Use cloud-ready session helper
            string dID1 = SessionHelper.GetValue<string>("dID");
            if (string.IsNullOrEmpty(dID1))
            {
                Response.Redirect("~/Patient/ViewDoctors.aspx");
                return;
            }

            int dID = Convert.ToInt32(dID1);

            int? pID = SessionHelper.UserId;
            if (!pID.HasValue)
            {
                Response.Redirect("~/SignUp.aspx");
                return;
            }

            int status = objmyDAl.getFreeSlots(dID, pID.Value, ref DT);

            if (status == -1)
            {
                PAppointment.Text = "There was some error in retrieving the Doctors's Free Slots.";
            }
            else if (status == 0)
            {
                PAppointment.Text = "There is currently no free slot of this doctor.";
            }
            else if (status > 0)
            {
                PAppointment.Text = "The following are the " + status  + " free slots of this doctor for today :";
                PAppointmentGrid.DataSource = DT;
                PAppointmentGrid.DataBind();
            }

            return;
        }
    }
}
