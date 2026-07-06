using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DBProject.DAL;
using DBProject;
using System.Data;



namespace DBProject.Doctor
{
    public partial class PreviousHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PatHistory(sender, e);
        }


        //-----------------------Function1--------------------------//

        protected void PatHistory(object sender, EventArgs e)
        {
            myDAL objmyDAl = new myDAL();

            DataTable DT = new DataTable();

            // Cloud-ready: Session accessed via CloudSessionHelper for Amazon ElastiCache for Redis
            // distributed session state (cr-dotnet-0045, cr-dotnet-0126)
            int id = CloudSessionHelper.GetInt(Session, "idoriginal");

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

            return;
        }


        //-----------------------Add a new function here------------------//


    }
}
