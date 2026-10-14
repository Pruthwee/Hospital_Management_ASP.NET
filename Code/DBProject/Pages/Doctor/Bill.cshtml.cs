using System;
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace doctor
{
    public partial class billModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public billModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string Label1 { get; set; } = "";

        public void OnGet()
        {
            myDAL objmyDAL = new myDAL();
            DataTable dt = new DataTable();
            int found;

            int did = (int)_httpContextAccessor.HttpContext.Session.GetInt32("idoriginal");

            found = objmyDAL.generate_bill_DAL(did, ref dt);

            if (found != 1)
            {
                Label1 = "<script>alert('There was some error');</script>";
            }
            else
            {
                Label1 = dt.Rows[0][0].ToString();
            }
        }

        public IActionResult OnPostBillPaid()
        {
            myDAL objmyDAL = new myDAL();

            int did = (int)_httpContextAccessor.HttpContext.Session.GetInt32("idoriginal");
            int appoint = (int)_httpContextAccessor.HttpContext.Session.GetInt32("appointid");
            objmyDAL.paid_bill_DAL(did, appoint);

            return Redirect("~/Pages/Doctor/PatientHistory.cshtml");
        }

        public IActionResult OnPostBillUnpaid()
        {
            myDAL objmyDAL = new myDAL();

            int did = (int)_httpContextAccessor.HttpContext.Session.GetInt32("idoriginal");
            int appoint = (int)_httpContextAccessor.HttpContext.Session.GetInt32("appointid");
            objmyDAL.Unpaid_bill_DAL(did, appoint);

            return Redirect("~/Pages/Doctor/PatientHistory.cshtml");
        }
    }
}
