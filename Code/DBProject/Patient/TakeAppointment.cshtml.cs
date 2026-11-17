using System;
using System.Data;
using DBProject.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DBProject
{
    public class TakeAppointmentModel : PageModel
    {
        public string TDept = "";

        // Data of the grid (null when it was not bound, like the old unbound GridView)
        public DataTable TDeptGrid = null;

        // Page_Load ran on every request (also on postbacks)
        public void OnGet()
        {
            pageLoad();
        }

        private void pageLoad()
        {
            HttpContext.Session.SetString("deptOriginal", "");
            deptInfo();
        }

        //---------------Function Called whenever a Department is selected from the Grid View----//
        public IActionResult OnPostSelect(int rowIndex)
        {
            pageLoad();

            Int16 num = Convert.ToInt16(rowIndex);

            // Cells[2] of the old grid = first data column (Select button and No. come before it)
            string deptName = Convert.ToString(TDeptGrid.Rows[num][0]);

            HttpContext.Session.SetString("deptOriginal", deptName);

            return Redirect("ViewDoctors.aspx");
        }



        //-----------------------Function1--------------------------//

        protected void deptInfo()
        {
            myDAL objmyDAl = new myDAL();

            DataTable DT = new DataTable();


            int status = objmyDAl.getdeptInfo(ref DT);


            if (status == -1)
            {
                TDept = "There was some error in retrieving the Departments Information.";
            }

            else
            {
                TDept = "Following are the departments available at our Clinic : ";
                TDeptGrid = DT;
            }

            return;
        }
    }
}
