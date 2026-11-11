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
    public partial class TreatmentHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RegisterAsyncTask(new PageAsyncTask(treatmentHistoryAsync));
            }
        }


        //-----------------------Function1--------------------------//

        protected async Task treatmentHistoryAsync()
        {
            await Task.Run(() =>
            {
                myDAL objmyDAl = new myDAL();

                DataTable DT = new DataTable();


                int id = (int)Session["idoriginal"];


                int status = objmyDAl.getTreatmentHistory(id, ref DT);


                if (status == -1)
                {
                    THistory.Text = "There was some error in retrieving the Patient's Treatment History.";
                }

                else if (status == 0)
                {
                    THistory.Text = "There is currently no treatment history of yours.";
                }

                else
                {
                    THistory.Text = "Treatment History of " + status + " Appointment(s) is found: ";
                    THistoryGrid.DataSource = DT;
                    THistoryGrid.DataBind();
                }
            });

            return;
        }

        protected void treatmentHistory(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(treatmentHistoryAsync));
        }
        

        //-----------------------Add a new function here------------------//




    }
}