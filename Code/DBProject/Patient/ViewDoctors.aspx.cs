using System;
// Cloud Readiness: Migrate to ASP.NET Core Razor Pages (cr-dotnet-0026)
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            Session["dID"] = "";
            if (!IsPostBack)
            {
                RegisterAsyncTask(new PageAsyncTask(deptDoctorInfoAsync));
            }
        }

        //---------------Function Called whenever a Doctor is selected from the Grid View----//
        protected void TDoctorGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                Int16 num = Convert.ToInt16(e.CommandArgument);

                string dID = TDoctorGrid.Rows[num].Cells[2].Text;
  
                Session["dID"] = dID;

                Response.BufferOutput = true;
                Response.Redirect("DoctorProfile.aspx");

                return;
            }
        }



        //-----------------------Function1--------------------------//

        protected async Task deptDoctorInfoAsync()
        {
            await Task.Run(() =>
            {
                myDAL objmyDAl = new myDAL();

                DataTable DT = new DataTable();

                string deptName = (string) Session["deptOriginal"];

                int status = objmyDAl.getDeptDoctorInfo(deptName, ref DT);


                if (status == -1)
                {
                    TDoctor.Text = "There was some error in retrieving the Doctors Information.";
                }

                else
                {
                    TDoctor.Text = "Following are our Specialized Doctors of " + Session["deptOriginal"] + " Department:";
                    TDoctorGrid.DataSource = DT;
                    TDoctorGrid.DataBind();
                }
            });

            return;
        }

        protected void deptDoctorInfo(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(deptDoctorInfoAsync));
        }


        //-----------------------Add a new function here------------------//
    }
}