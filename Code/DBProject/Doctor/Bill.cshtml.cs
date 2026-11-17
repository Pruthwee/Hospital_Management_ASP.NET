using System;
using System.Data;
using DBProject;
using DBProject.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace doctor
{
    public class billModel : PageModel
    {
        public string Label1 = "Label";

        // old Page_Load (ran on every request)
        private void LoadBill()
        {
            myDAL objmyDAL = new myDAL();
            DataTable dt = new DataTable();
            int found;

            int did = HttpContext.Session.GetRequiredInt32("idoriginal");

            found = objmyDAL.generate_bill_DAL(did, ref dt);

            if (found != 1)
            { ViewData["Alert"] = "There was some error"; }
            else
            {
                Label1 = dt.Rows[0][0].ToString();
            }
        }

        public void OnGet()
        {
            LoadBill();
        }

        public IActionResult OnPostPaid()
        {
            LoadBill();

            myDAL objmyDAL = new myDAL();

            int did = HttpContext.Session.GetRequiredInt32("idoriginal");
            int appoint = HttpContext.Session.GetRequiredInt32("appointid");
            objmyDAL.paid_bill_DAL(did, appoint);

            return Redirect("patienthistory.aspx");
        }

        public IActionResult OnPostUnpaid()
        {
            LoadBill();

            myDAL objmyDAL = new myDAL();

            int did = HttpContext.Session.GetRequiredInt32("idoriginal");
            int appoint = HttpContext.Session.GetRequiredInt32("appointid");
            objmyDAL.Unpaid_bill_DAL(did, appoint);

            return Redirect("patienthistory.aspx");
        }
    }
}
