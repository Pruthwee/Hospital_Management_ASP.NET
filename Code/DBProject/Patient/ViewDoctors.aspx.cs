using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DBProject.DAL;
using System.Data;


namespace DBProject
{
    public partial class ViewDoctors : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Cloud-ready: Session set via CloudSessionHelper for Amazon ElastiCache for Redis
            // distributed session state (cr-dotnet-0045, cr-dotnet-0126)
            CloudSessionHelper.Set(Session, "dID", "");
            deptDoctorInfo(sender, e);
        }

        //---------------Function Called whenever a Doctor is selected from the Grid View----//
        protected void TDoctorGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                Int16 num = Convert.ToInt16(e.CommandArgument);

                string dID = TDoctorGrid.Rows[num].Cells[2].Text;
  
                // Cloud-ready: Session set via CloudSessionHelper for distributed session state
                CloudSessionHelper.Set(Session, "dID", dID);

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

            // Cloud-ready: Session accessed via CloudSessionHelper for distributed session state
            string deptName = CloudSessionHelper.GetString(Session, "deptOriginal");

            int status = objmyDAl.getDeptDoctorInfo(deptName, ref DT);


            if (status == -1)
            {
                TDoctor.Text = "There was some error in retrieving the Doctors Information.";
            }

            else
            {
                TDoctor.Text = "Following are our Specialized Doctors of " + CloudSessionHelper.GetString(Session, "deptOriginal") + " Department:";
                TDoctorGrid.DataSource = DT;
                TDoctorGrid.DataBind();
            }

            return;
        }


        //-----------------------Add a new function here------------------//
    }
}
