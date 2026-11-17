using System;
using System.Data;
using DBProject;
using DBProject.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace doctor
{
    public class patienthistoryModel : PageModel
    {
        // GridView data source (null when the DAL reported an error)
        public DataTable Patientsgrid { get; set; }

        // old Page_Load (ran on every request)
        private int LoadGrid()
        {
            myDAL objmydal = new myDAL();
            DataTable dt = new DataTable();
            int found = 0;

            int did = HttpContext.Session.GetRequiredInt32("idoriginal");

            found = objmydal.search_patient_DAL(did, ref dt);
            if (found != 1)
            { ViewData["Alert"] = "There was some error"; }
            else
            {
                Patientsgrid = dt;
            }
            return found;
        }

        public void OnGet()
        {
            LoadGrid();
        }

        // old patientsgrid_RowCommand (CommandName == "Select")
        public IActionResult OnPostSelect(int rowIndex)
        {
            LoadGrid();

            Int16 num = Convert.ToInt16(rowIndex);

            string aId = Patientsgrid.Rows[num][0].ToString();

            //retrieve appointmentid  from that row (key-non editable)
            int appointmentid = Convert.ToInt32(aId);

            HttpContext.Session.SetInt32("appointid", appointmentid);
            return Redirect("Historyupdate.aspx");
        }
    }
}
