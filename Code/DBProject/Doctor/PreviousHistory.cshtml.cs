using System;
using System.Data;
using DBProject;
using DBProject.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DBProject.Doctor
{
    public class PreviousHistoryModel : PageModel
    {
        public string PHistory = "";

        // GridView data source (null when there was an error)
        public DataTable PHistoryGrid { get; set; }

        public void OnGet()
        {
            PatHistory();
        }

        //-----------------------Function1--------------------------//

        protected void PatHistory()
        {
            myDAL objmyDAl = new myDAL();

            DataTable DT = new DataTable();


            int id = HttpContext.Session.GetRequiredInt32("idoriginal");


            int status = objmyDAl.getPHistory(id, ref DT);


            if (status == -1)
            {
                PHistory = "There was some error in retrieving the Patients History.";
            }


            else
            {
                PHistoryGrid = DT;
            }

            return;
        }
    }
}
