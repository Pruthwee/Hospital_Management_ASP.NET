using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DBProject.DAL;
using DBProject.Helpers;
using System.Data;

namespace DBProject
{
    public partial class PatientHome : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check authentication using cloud-ready session helper
            if (!SessionHelper.IsAuthenticated)
            {
                Response.Redirect("~/SignUp.aspx");
                return;
            }

            patientInfo(sender,e);
        }

        //-----------------------Function1--------------------------//
        protected void patientInfo (object sender, EventArgs e)
        {
            myDAL objmyDAl = new myDAL();

            // Use cloud-ready session helper
            int? pid = SessionHelper.UserId;
            if (!pid.HasValue)
            {
                Response.Redirect("~/SignUp.aspx");
                return;
            }

            string name      = "";
            string phone     = "";
            string address   = "";
            string birthDate = "";
            int    age       = 0 ;
            string gender    = "";

            int status = objmyDAl.patientInfoDisplayer(pid.Value, ref name, ref phone, ref address, ref birthDate, ref age, ref gender);

            if (status == -1)
            {
                Response.Write("<script>alert('There was some error in retrieving the Patient's Info.');</script>");
            }
            else if (status == 0)
            {
                PName.Text      = name;
                PPhone.Text     = phone;
                PBirthDate.Text = birthDate;
                PatientAge.Text = age.ToString();
                PAddress.Text   = address;
                PGender.Text    = gender;
            }

            return;
        }
    }
}
