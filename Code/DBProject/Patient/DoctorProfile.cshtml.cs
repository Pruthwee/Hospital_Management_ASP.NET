using System;
using System.Data;
using DBProject.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DBProject
{
    public class DoctorProfileModel : PageModel
    {
        public string DName = "";
        public string DPhone = "";
        public string DQualification = "";
        public string DSpecialization = "";
        public string DWork = "";
        public string DAge = "";
        public string DGender = "";
        public string DDept = "";
        public string DCharges = "";
        public string DRI = "";
        public string DPT = "";

        // Page_Load ran on every request (also on postbacks)
        public void OnGet()
        {
            doctorInfo();
        }



        //-----------------------Function1--------------------------//

        protected void doctorInfo()
        {
            myDAL objmyDAl = new myDAL();

            string dID1 = HttpContext.Session.GetString("dID");

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

            string deptName = HttpContext.Session.GetString("deptOriginal");

            int status = objmyDAl.doctorInfoDisplayer(dID, ref name, ref phone, ref gender, ref charges_Per_Visit, ref ReputeIndex, ref PatientsTreated, ref qualification, ref specialization, ref workE, ref age);

            if (status == -1)
            {
                ViewData["Alert"] = "There was some error in retrieving the Doctor's Info.";
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

            return;
        }


        //-----------------------Function2------------------//

        public IActionResult OnPostRedirectToAppointmentTaker()
        {
            doctorInfo();
            return Redirect("AppointmentTaker.aspx");
        }
    }
}
