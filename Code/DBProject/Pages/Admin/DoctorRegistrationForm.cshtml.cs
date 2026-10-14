using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject
{
    public partial class DoctorRegistrationFormModel : PageModel
    {
        public List<DepartmentItem> DepartmentList { get; set; } = new List<DepartmentItem>();
        public string MsgText { get; set; } = "";
        public bool MsgVisible { get; set; } = false;
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string BirthDate { get; set; } = "";
        public string Department { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
        public string Exp { get; set; } = "";
        public string Salary { get; set; } = "";
        public string Charges_per_visit { get; set; } = "";
        public string spec { get; set; } = "";
        public string Qualification { get; set; } = "";

        public class DepartmentItem
        {
            public string Value { get; set; }
            public string Text { get; set; }
        }

        public void OnGet()
        {
            LoadDepartments();
        }

        public void OnPost(string Name, string BirthDate, string Email, string Password, string Department, string Phone, string Gender, string Address, string Exp, string Salary, string Charges_per_visit, string spec, string Qualification)
        {
            LoadDepartments();

            myDAL objmyDAL = new myDAL();

            int exp = Convert.ToInt32(Exp);
            int salary = Convert.ToInt32(Salary);
            int chargesPerVisit = Convert.ToInt32(Charges_per_visit);
            int dept = Convert.ToInt32(Department);
            string gender = Gender;

            objmyDAL.AddDoctor(Name, Email, Password, BirthDate, dept, Phone, gender[0], Address, exp, salary, chargesPerVisit, spec, Qualification);
            MsgVisible = true;
            MsgText = "doctor Added Succesfully";
            flushInformation();
        }

        private void LoadDepartments()
        {
            myDAL objmyDAL = new myDAL();
            DataTable dt = new DataTable();
            objmyDAL.getdeptInfo(ref dt);
            foreach (DataRow row in dt.Rows)
            {
                DepartmentList.Add(new DepartmentItem
                {
                    Value = row["DeptNo"].ToString(),
                    Text = row["DeptName"].ToString()
                });
            }
        }

        protected void flushInformation()
        {
            Name = "";
            Email = "";
            Password = "";
            BirthDate = "";
            Department = "";
            Phone = "";
            Address = "";
            Exp = "";
            Salary = "";
            Charges_per_visit = "";
            spec = "";
            Qualification = "";
        }
    }
}
