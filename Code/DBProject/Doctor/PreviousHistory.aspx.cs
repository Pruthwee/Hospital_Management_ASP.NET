using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DBProject.DAL;
using System.Data;
using System.Threading.Tasks;



namespace DBProject.Doctor
{
    public partial class PreviousHistory : System.Web.UI.Page
    {
        protected async void Page_Load(object sender, EventArgs e)
        {
            await PatHistoryAsync(sender, e);
        }


        //-----------------------Function1--------------------------//

        protected async Task PatHistoryAsync(object sender, EventArgs e)
        {
            myDAL objmyDAl = new myDAL();

            DataTable DT = new DataTable();


            int id = (int)Session["idoriginal"];


            int status = await Task.Run(() => objmyDAl.getPHistory(id, ref DT));


            if (status == -1)
            {
                PHistory.Text = "There was some error in retrieving the Patients History.";
            }

          
            else
            {
                PHistoryGrid.DataSource = DT;
                PHistoryGrid.DataBind();
            }

            return;
        }


        //-----------------------Add a new function here------------------//





    }
}