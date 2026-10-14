using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject
{
    public partial class SignUpModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SignUpModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void OnGet()
        {
            _httpContextAccessor.HttpContext.Session.SetString("idoriginal", "");
        }

        //-----------------------Function1--------------------------//
        public IActionResult OnPostLogin(string loginEmail, string loginPassword)
        {
            myDAL objmyDAl = new myDAL();

            int status = 0;
            int type = 0;
            int id = 0;

            status = objmyDAl.validateLogin(loginEmail, loginPassword, ref type, ref id);

            if (status == 0)
            {
                _httpContextAccessor.HttpContext.Session.SetInt32("idoriginal", id);

                if (type == 1)
                {
                    return Redirect("~/Pages/Patient/PatientHome.cshtml");
                }

                else if (type == 2)
                {
                    return Redirect("~/Pages/Doctor/DoctorHome.cshtml");
                }

                else if (type == 3)
                {
                    return Redirect("~/Pages/Admin/AdminHome.cshtml");
                }
            }

            else if (status == 1)
            {
                TempData["alert"] = "Email not found. Try Again !";
                return Page();
            }

            else if (status == 2)
            {
                TempData["alert"] = "Incorrect Password. Try Again !";
                return Page();
            }

            else if (status == -1)
            {
                TempData["alert"] = "There was some error. Try Again !";
                return Page();
            }

            return Page();
        }

        //-----------------------Function2--------------------------//
        public IActionResult OnPostSignup(string sName, string sBirthDate, string sEmail, string sPassword, string Phone, string Address, string Gender)
        {
            string Name = sName;
            string BirthDate = sBirthDate;
            string Email = sEmail;
            string Password = sPassword;
            string PhoneNo = Phone;
            string Addr = Address;

            string gender = Gender;

            myDAL objmyDAl = new myDAL();

            int id = 0;

            int status = objmyDAl.validateUser(Name, BirthDate, Email, Password, PhoneNo, gender, Addr, ref id);

            //status == 0 failure
            if (status == 0)
            {
                TempData["alert"] = "Email already exists. Please choose a different one.";
                return Page();
            }

            else if (status == 1)
            {
                _httpContextAccessor.HttpContext.Session.SetInt32("idoriginal", id);
                return Redirect("~/Pages/Patient/PatientHome.cshtml");
            }

            else if (status == -1)
            {
                TempData["alert"] = "There was some error. Try again !";
                return Page();
            }

            return Page();
        }
    }
}
