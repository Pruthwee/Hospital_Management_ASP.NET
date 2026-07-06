using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DBProject.DAL;
using DBProject;
using System.Data;

namespace doctor
{
    public partial class bill : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            myDAL objmyDAL = new myDAL();
            DataTable dt = new DataTable();
            int found;

            // Cloud-ready: Session accessed via CloudSessionHelper for Amazon ElastiCache for Redis
            // distributed session state (cr-dotnet-0045, cr-dotnet-0126)
            int did = CloudSessionHelper.GetInt(Session, "idoriginal");
            
            found = objmyDAL.generate_bill_DAL(did, ref dt);

            if (found != 1)
            { Response.Write("<script>alert('There was some error');</script>"); }
            else
            {
                Label1.Text = dt.Rows[0][0].ToString();
            }
        }


        public void bill_paid(object sender, EventArgs e)
        {
            myDAL objmyDAL = new myDAL();
            
            // Cloud-ready: Session accessed via CloudSessionHelper for Amazon ElastiCache for Redis
            // distributed session state (cr-dotnet-0045, cr-dotnet-0126)
            int did = CloudSessionHelper.GetInt(Session, "idoriginal");
            int appoint = CloudSessionHelper.GetInt(Session, "appointid");
            objmyDAL.paid_bill_DAL(did, appoint);

			Response.BufferOutput = false;
            Response.Redirect("patienthistory.aspx");
        }


        public void bill_Unpaid(object sender, EventArgs e)
        {
            myDAL objmyDAL = new myDAL();

            // Cloud-ready: Session accessed via CloudSessionHelper for Amazon ElastiCache for Redis
            // distributed session state (cr-dotnet-0045, cr-dotnet-0126)
            int did = CloudSessionHelper.GetInt(Session, "idoriginal");
            int appoint = CloudSessionHelper.GetInt(Session, "appointid");
            objmyDAL.Unpaid_bill_DAL(did, appoint);

            Response.BufferOutput = false;
            Response.Redirect("patienthistory.aspx");
        }
    }
}
