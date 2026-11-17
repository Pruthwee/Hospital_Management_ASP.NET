using System;
using System.Data;
using DBProject;
using DBProject.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace doctor
{
    public class HistoryupdateModel : PageModel
    {
        [BindProperty]
        public string Disease { get; set; }

        [BindProperty]
        public string progress { get; set; }

        [BindProperty]
        public string Prescription { get; set; }

        public void OnGet()
        {

        }

        public IActionResult OnPostSave()
        {
            myDAL objmyDAL = new myDAL();
            int found;
            int did = HttpContext.Session.GetRequiredInt32("idoriginal");
            string disease = Disease ?? "";
            string progres = progress ?? "";
            string prescrip = Prescription ?? "";

            int appid = HttpContext.Session.GetRequiredInt32("appointid");


            found = objmyDAL.update_prescription_DAL(did, appid, disease, progres, prescrip);

            if (found != 1)
            { ViewData["Alert"] = "There was some error"; }
            else
            {
                { ViewData["Alert"] = "Information Successfully Updated"; }
            }
            return Page();
        }


        public IActionResult OnPostGenerateBill()
        {
            return Redirect("bill.aspx");
        }
    }
}
