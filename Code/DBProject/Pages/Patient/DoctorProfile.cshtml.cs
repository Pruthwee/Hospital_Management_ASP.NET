using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject
{
    public partial class DoctorProfileModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DoctorProfileModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string DName { get; set; } = "";
        public string DPhone { get; set; } = "";
        public string DQualification { get; set; } = "";
        public string DSpecialization { get; set; } = "";
        public string DWork { get; set; } = "";
        public string DAge { get; set; } = "";
        public string DGender { get; set; } = "";
        public string DDept { get; set; } = "";
        public string DCharges { get; set; } = "";
        public string DRI { get; set; } = "";
        public string DPT { get; set; } = "";

        public void OnGet()
        {
            doctorInfo();
        }

        protected void doctorInfo()
        {
            myDAL objmyDAl = new myDAL();

            string dID1 = _httpContextAccessor.HttpContext.Session.GetString("dID");

            int dID = Convert.ToInt32(dID1);

            string name = "";
            string phone = "";
            string gender = "";

            float charges_Per_Visit = 0;
            float ReputeIndex = 0;
            int PatientsTreated = 0;
            string qualification = "";
            string specialization = "";
            int workE = 0;
            int age = 0;

            string deptName = _httpContextAccessor.HttpContext.Session.GetString("deptOriginal");

            int status = objmyDAl.doctorInfoDisplayer(dID, ref name, ref phone, ref gender, ref charges_Per_Visit, ref ReputeIndex, ref PatientsTreated, ref qualification, ref specialization, ref workE, ref age);

            if (status == -1)
            {
                TempData["alert"] = "There was some error in retrieving the Doctor's Info.";
            }
            else if (status == 0)
            {
                DName = name;
                DPhone = phone;
                DQualification = qualification;
                DSpecialization = specialization;
                DWork = workE.ToString();
                DAge = age.ToString();
                DGender = gender;
                DDept = deptName;
                DCharges = charges_Per_Visit.ToString();
                DRI = ReputeIndex.ToString();
                DPT = PatientsTreated.ToString();
            }
        }

        public IActionResult OnPostRedirectToAppointmentTaker()
        {
            return Redirect("~/Pages/Patient/AppointmentTaker.cshtml");
        }
    }
}
