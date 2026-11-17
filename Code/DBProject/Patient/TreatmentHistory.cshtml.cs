using System;
using System.Data;
using DBProject.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DBProject
{
    public class TreatmentHistoryModel : PageModel
    {
        public string THistory { get; set; }

        // Data of the old THistoryGrid (null = grid not bound, nothing rendered)
        public DataTable THistoryGrid { get; set; }

        public void OnGet()
        {
            treatmentHistory();
        }

        //-----------------------Function1--------------------------//

        protected void treatmentHistory()
        {
            myDAL objmyDAl = new myDAL();

            DataTable DT = new DataTable();


            int id = HttpContext.Session.GetRequiredInt32("idoriginal");


            int status = objmyDAl.getTreatmentHistory(id, ref DT);


            if (status == -1)
            {
                THistory = "There was some error in retrieving the Patient's Treatment History.";
            }

            else if (status == 0)
            {
                THistory = "There is currently no treatment history of yours.";
            }

            else
            {
                THistory = "Treatment History of " + status + " Appointment(s) is found: ";
                THistoryGrid = DT;
            }

            return;
        }
    }
}
