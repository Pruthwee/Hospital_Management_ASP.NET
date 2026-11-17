using System;
using System.Data;
using DBProject.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DBProject
{
    public class PatientHomeModel : PageModel
    {
        public string PName { get; set; }
        public string PPhone { get; set; }
        public string PBirthDate { get; set; }
        public string PatientAge { get; set; }
        public string PGender { get; set; }
        public string PAddress { get; set; }

        public void OnGet()
        {
            patientInfo();
        }

        //-----------------------Function1--------------------------//

        protected void patientInfo()
        {
            myDAL objmyDAl = new myDAL();

            int pid = HttpContext.Session.GetRequiredInt32("idoriginal");

            string name      = "";
            string phone     = "";
            string address   = "";
            string birthDate = "";
            int    age       = 0 ;
            string gender    = "";


            int status = objmyDAl.patientInfoDisplayer(pid, ref name, ref phone, ref address, ref birthDate, ref age, ref gender);

            if (status == -1)
            {
                ViewData["Alert"] = "There was some error in retrieving the Patient's Info.";
            }

            else if (status == 0)
            {
                PName      = name;
                PPhone     = phone;
                PBirthDate = birthDate;
                PatientAge = age.ToString();
                PAddress   = address;
                PGender    = gender;
            }

            return;
        }
    }
}
