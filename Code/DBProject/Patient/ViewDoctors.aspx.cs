using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DBProject.DAL;
using DBProject.CloudInfrastructure;
using System.Data;


namespace DBProject
{
    public partial class ViewDoctors : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Use distributed session manager for cloud-ready session handling
            DistributedSessionManager.SetSessionValue("dID", "");
            deptDoctorInfo(sender, e);
        }

        //---------------Function Called whenever a Doctor is selected from the Grid View----//
        protected void TDoctorGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                Int16 num = Convert.ToInt16(e.CommandArgument);

                string dID = TDoctorGrid.Rows[num].Cells[2].Text;
  
                // Store in distributed session for horizontal scaling
                DistributedSessionManager.SetSessionValue("dID", dID);

                Response.BufferOutput = true;
                Response.Redirect("DoctorProfile.aspx");

                return;
            }
        }



        //-----------------------Function1--------------------------//

        protected void deptDoctorInfo(object sender, EventArgs e)
        {
            myDAL objmyDAl = new myDAL();

            DataTable DT = new DataTable();

            // Retrieve from distributed session
            string deptName = DistributedSessionManager.GetSessionValue<string>("deptOriginal", "");

            int status = objmyDAl.getDeptDoctorInfo(deptName, ref DT);


            if (status == -1)
            {
                TDoctor.Text = "There was some error in retrieving the Doctors Information.";
            }

            else
            {
                string deptOriginal = DistributedSessionManager.GetSessionValue<string>("deptOriginal", "");
                TDoctor.Text = "Following are our Specialized Doctors of " + deptOriginal + " Department:";
                TDoctorGrid.DataSource = DT;
                TDoctorGrid.DataBind();
            }

            return;
        }


        //-----------------------Add a new function here------------------//
    }
}
