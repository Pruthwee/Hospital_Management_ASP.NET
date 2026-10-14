using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject
{
    public partial class AddStaffModel : PageModel
    {
        public string MsgText { get; set; } = "";
        public bool MsgVisible { get; set; } = false;
        public string Name { get; set; } = "";
        public string BirthDate { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
        public string Salary { get; set; } = "";
        public string Qual { get; set; } = "";
        public string Designation { get; set; } = "";

        public void OnGet()
        {
        }

        protected void StaffRegister(string Name, string BirthDate, string Phone, string Gender, string Address, string Salary, string Qual, string Designation)
        {
            myDAL objmyDAL = new myDAL();

            int salary = Convert.ToInt32(Salary);
            string gender = Gender;

            if (objmyDAL.AddStaff(Name, BirthDate, Phone, gender[0], Address, salary, Qual, Designation) == 1) ;
            {
                MsgVisible = true;
                MsgText = Designation + " Added Succesfully";
                flushInformation();
            }
        }

        public IActionResult OnPost(string Name, string BirthDate, string Phone, string Gender, string Address, string Salary, string Qual, string Designation)
        {
            StaffRegister(Name, BirthDate, Phone, Gender, Address, Salary, Qual, Designation);
            return Page();
        }

        protected void flushInformation()
        {
            Name = "";
            BirthDate = "";
            Phone = "";
            Address = "";
            Salary = "";
            Qual = "";
            Designation = "";
        }
    }
}
