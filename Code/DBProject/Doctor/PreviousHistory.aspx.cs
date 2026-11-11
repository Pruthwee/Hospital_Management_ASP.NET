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



namespace DBProject.Doctor
{
    public partial class PreviousHistory : System.Web.UI.Page
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
                myDAL objmyDAl = new myDAL();

                DataTable DT = new DataTable();


                int id = (int)Session["idoriginal"];


                int status = objmyDAl.getPHistory(id, ref DT);


                if (status == -1)
                {
                    PHistory.Text = "There was some error in retrieving the Patients History.";
                }

          
                else
                {
                    PHistoryGrid.DataSource = DT;
                    PHistoryGrid.DataBind();
                }
            });

            return;
        }

        protected void PatHistory(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(PatHistoryAsync));
        }


        //-----------------------Add a new function here------------------//





    }
}