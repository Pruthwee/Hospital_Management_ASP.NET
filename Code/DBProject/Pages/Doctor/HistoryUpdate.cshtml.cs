using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace doctor
{
    public partial class HistoryupdateModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HistoryupdateModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string Disease { get; set; } = "";
        public string progress { get; set; } = "";
        public string Prescription { get; set; } = "";

        public void OnGet()
        {
        }

        public IActionResult OnPostSave(string Disease, string progress, string Prescription)
        {
            myDAL objmyDAL = new myDAL();
            int found;
            int did = (int)_httpContextAccessor.HttpContext.Session.GetInt32("idoriginal");
            string disease = Disease;
            string progres = progress;
            string prescrip = Prescription;

            int appid = (int)_httpContextAccessor.HttpContext.Session.GetInt32("appointid");

            found = objmyDAL.update_prescription_DAL(did, appid, disease, progres, prescrip);

            if (found != 1)
            {
                TempData["alert"] = "There was some error";
            }
            else
            {
                TempData["alert"] = "Information Successfully Updated";
            }

            return Redirect("~/Pages/Doctor/HistoryUpdate.cshtml");
        }

        public IActionResult OnPostGenerateBill()
        {
            return Redirect("~/Pages/Doctor/Bill.cshtml");
        }
    }
}
