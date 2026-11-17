using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;
using System.Data;


namespace DBProject
{
    public class SignUpModel : PageModel
    {
        [BindProperty]
        public string loginEmail { get; set; }

        [BindProperty]
        public string loginPassword { get; set; }

        [BindProperty]
        public string sName { get; set; }

        [BindProperty]
        public string sBirthDate { get; set; }

        [BindProperty]
        public string sEmail { get; set; }

        [BindProperty]
        public string sPassword { get; set; }

        [BindProperty]
        public string scPassword { get; set; }

        [BindProperty]
        public string Phone { get; set; }

        [BindProperty]
        public string Address { get; set; }

        // Page_Load
        public void OnGet()
        {
            HttpContext.Session.SetString("idoriginal", "");
        }

        //-----------------------Function1--------------------------//
        public IActionResult OnPostLoginV()
        {
            // Page_Load ran on every postback too
            HttpContext.Session.SetString("idoriginal", "");

            string email = loginEmail ?? "";
            string password = loginPassword ?? "";

            myDAL objmyDAl = new myDAL();

            int status = 0;
            int type = 0;
            int id = 0;

            status = objmyDAl.validateLogin(email, password, ref type, ref id);

            if (status == 0)
            {
                HttpContext.Session.SetInt32("idoriginal", id);

                if (type == 1)
                {
                    return Redirect("/Patient/PatientHome.aspx");
                }

                else if (type == 2)
                {
                    return Redirect("/Doctor/DoctorHome.aspx");
                }

                else if (type == 3)
                {
                    return Redirect("/Admin/AdminHome.aspx");
                }
            }


            else if (status == 1)
            {
                ViewData["Alert"] = "Email not found. Try Again !";
            }

            else if (status == 2)
            {
                ViewData["Alert"] = "Incorrect Password. Try Again !";
            }

            else if (status == -1)
            {
                ViewData["Alert"] = "There was some error. Try Again !";
            }

            return Page();
        }




        //-----------------------Function2--------------------------//
        public IActionResult OnPostSignupV()
        {
            // Page_Load ran on every postback too
            HttpContext.Session.SetString("idoriginal", "");

            string Name = sName ?? "";
            string BirthDate = sBirthDate ?? "";
            string Email = sEmail ?? "";
            string Password = sPassword ?? "";
            string PhoneNo = Phone ?? "";
            string Addr = Address ?? "";

            string gender = Request.Form["Gender"].ToString();



            myDAL objmyDAl = new myDAL();

            int id = 0;

            int status = objmyDAl.validateUser(Name, BirthDate, Email, Password, PhoneNo, gender, Addr, ref id);


            //status == 0 failure
            if (status == 0)
            {
                ViewData["Alert"] = "Email already exists. Please choose a different one.";
            }

            else if (status == 1)
            {
                HttpContext.Session.SetInt32("idoriginal", id);

                //Response.Write("<script>alert('Registration Successful !');</script>");

                return Redirect("/Patient/PatientHome.aspx");
            }

            else if (status == -1)
            {
                ViewData["Alert"] = "There was some error. Try again !";
            }

            return Page();
        }

        //Enter new function here//
    }

}
