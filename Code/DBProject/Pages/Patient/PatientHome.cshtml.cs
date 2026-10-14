using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject
{
    public partial class PatientHomeModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PatientHomeModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string PName { get; set; } = "";
        public string PPhone { get; set; } = "";
        public string PBirthDate { get; set; } = "";
        public string PatientAge { get; set; } = "";
        public string PGender { get; set; } = "";
        public string PAddress { get; set; } = "";

        public void OnGet()
        {
            patientInfo();
        }

        protected void patientInfo()
        {
            myDAL objmyDAl = new myDAL();

            int pid = (int)_httpContextAccessor.HttpContext.Session.GetInt32("idoriginal");

            string name = "";
            string phone = "";
            string address = "";
            string birthDate = "";
            int age = 0;
            string gender = "";

            int status = objmyDAl.patientInfoDisplayer(pid, ref name, ref phone, ref address, ref birthDate, ref age, ref gender);

            if (status == -1)
            {
                TempData["alert"] = "There was some error in retrieving the Patient's Info.";
            }
            else if (status == 0)
            {
                PName = name;
                PPhone = phone;
                PBirthDate = birthDate;
                PatientAge = age.ToString();
                PAddress = address;
                PGender = gender;
            }
        }
    }
}
