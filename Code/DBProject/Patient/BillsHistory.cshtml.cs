using System;
using System.Data;
using DBProject.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DBProject
{
    public class BillsHistoryModel : PageModel
    {
        public string BHistory { get; set; }

        // Data of the old BHistoryGrid (null = grid not bound, nothing rendered)
        public DataTable BHistoryGrid { get; set; }

        public void OnGet()
        {
            billHistory();
        }

        //-----------------------Function1--------------------------//

        protected void billHistory()
        {
            myDAL objmyDAl = new myDAL();

            DataTable DT = new DataTable();


            int id = HttpContext.Session.GetRequiredInt32("idoriginal");


            int status = objmyDAl.getBillHistory(id, ref DT);


            if (status == -1)
            {
                BHistory = "There was some error in retrieving the Patient's Bill History.";
            }

            else if (status == 0)
            {
                BHistory = "There is currently no bill history of yours.";
            }

            else
            {
                BHistory = status + " Bill(s) are found: ";
                BHistoryGrid = DT;
            }

            return;
        }
    }
}
