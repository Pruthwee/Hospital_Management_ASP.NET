using System;
using System.Data;
using DBProject.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DBProject
{
    public class ViewDoctorsModel : PageModel
    {
        public string TDoctor = "";

        // Data of the grid (null when it was not bound, like the old unbound GridView)
        public DataTable TDoctorGrid = null;

        // Page_Load ran on every request (also on postbacks)
        public void OnGet()
        {
            pageLoad();
        }

        private void pageLoad()
        {
            HttpContext.Session.SetString("dID", "");
            deptDoctorInfo();
        }

        //---------------Function Called whenever a Doctor is selected from the Grid View----//
        public IActionResult OnPostSelect(int rowIndex)
        {
            pageLoad();

            Int16 num = Convert.ToInt16(rowIndex);

            // Cells[2] of the old grid = first data column (Select button and No. come before it)
            string dID = Convert.ToString(TDoctorGrid.Rows[num][0]);

            HttpContext.Session.SetString("dID", dID);

            return Redirect("DoctorProfile.aspx");
        }



        //-----------------------Function1--------------------------//

        protected void deptDoctorInfo()
        {
            myDAL objmyDAl = new myDAL();

            DataTable DT = new DataTable();

            string deptName = HttpContext.Session.GetString("deptOriginal");

            int status = objmyDAl.getDeptDoctorInfo(deptName, ref DT);


            if (status == -1)
            {
                TDoctor = "There was some error in retrieving the Doctors Information.";
            }

            else
            {
                TDoctor = "Following are our Specialized Doctors of " + HttpContext.Session.GetString("deptOriginal") + " Department:";
                TDoctorGrid = DT;
            }

            return;
        }
    }
}
