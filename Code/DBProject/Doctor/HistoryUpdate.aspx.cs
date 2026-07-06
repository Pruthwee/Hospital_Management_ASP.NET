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
    public partial class Historyupdate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void saveindatabase(object sender, EventArgs e)
        {
            myDAL objmyDAL = new myDAL();
            int found;

            // Cloud-ready: Session accessed via CloudSessionHelper for Amazon ElastiCache for Redis
            // distributed session state (cr-dotnet-0045, cr-dotnet-0126)
            int did = CloudSessionHelper.GetInt(Session, "idoriginal");
            string disease = Disease.Text;
            string progres = progress.Text;
            string prescrip = Prescription.Text;

            int appid = CloudSessionHelper.GetInt(Session, "appointid");

            
            found = objmyDAL.update_prescription_DAL(did, appid, disease, progres, prescrip);

            if (found != 1)
            { Response.Write("<script>alert('There was some error');</script>"); }
            else
            {
                { Response.Write("<script>alert('Information Successfully Updated');</script>"); }
            }
        }


        public void generate_bill(object sender, EventArgs e)
        {
            Response.Redirect("bill.aspx");
        }
    }
}
