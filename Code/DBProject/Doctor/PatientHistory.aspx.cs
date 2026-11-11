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


namespace doctor
{
    public partial class patienthistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
                if (!IsPostBack)
                {
                        RegisterAsyncTask(new PageAsyncTask(PatHistoryAsync));
                }
        }


        //-----------------------Function1--------------------------//

        protected async Task PatHistoryAsync()
        {
                await Task.Run(() =>
                {
                        myDAL objmydal = new myDAL();
                        DataTable dt = new DataTable();
                        int found = 0;

                        int did = (int)Session["idoriginal"];

                        found = objmydal.search_patient_DAL(did, ref dt);
                        if (found != 1)
                        { Response.Write("<script>alert('There was some error');</script>"); }
                        else
                        {
                                patientsgrid.DataSource = dt;
                                patientsgrid.DataBind();
                        }
                });
        }

        protected void PatHistory(object sender, EventArgs e)
        {
                RegisterAsyncTask(new PageAsyncTask(PatHistoryAsync));
        }

        
        protected void patientsgrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {

                Int16 num = Convert.ToInt16(e.CommandArgument);

                string aId = patientsgrid.Rows[num].Cells[1].Text;

                //retrieve appointmentid  from that row (key-non editable)
                int appointmentid = Convert.ToInt32(aId);

                Session["appointid"] = appointmentid;
                Response.Redirect("Historyupdate.aspx");
            }
        }
    }

}