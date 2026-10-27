using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using DBProject.DAL;

namespace DBProject.Admin
{
    /// <summary>
    /// Razor Page model for Doctor Registration — replaces ASP.NET Web Forms DoctorRegistrationForm.aspx / DoctorRegistrationForm.aspx.cs.
    /// Eliminates ViewState, postbacks, and server affinity; enables stateless horizontal scaling on Azure Container Apps.
    /// </summary>
    public class DoctorRegistrationFormModel : PageModel
    {
        [BindProperty]
        public DoctorRegistrationInputModel Input { get; set; } = new();

        public string Message { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            // Validate department selection
            if (Input.Department == 0)
            {
                ModelState.AddModelError("Input.Department", "Please Select Department");
            }

            // Validate email uniqueness
            var objmyDAL = new myDAL();
            if (objmyDAL.DoctorEmailAlreadyExist(Input.Email) == 1)
            {
                ModelState.AddModelError("Input.Email", "This Email Already exists, kindly choose a different one!");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            string gender = Request.Form["Gender"].ToString();
            if (string.IsNullOrEmpty(gender))
            {
                gender = "M";
            }

            int exp = int.Parse(Input.Exp);
            int salary = int.Parse(Input.Salary);
            int chargesPerVisit = int.Parse(Input.ChargesPerVisit);

            objmyDAL.AddDoctor(
                Input.Name,
                Input.Email,
                Input.Password,
                Input.BirthDate,
                Input.Department,
                Input.Phone,
                gender[0],
                Input.Address,
                exp,
                salary,
                chargesPerVisit,
                Input.Specialization,
                Input.Qualification);

            Message = "Doctor Added Successfully";
            Input = new DoctorRegistrationInputModel();
            return Page();
        }
    }

    public class DoctorRegistrationInputModel
    {
        [Required(ErrorMessage = "* Required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "*Required")]
        [RegularExpression(@"((?:0[1-9])|(?:1[0-2]))\/((?:0[0-9])|(?:[1-2][0-9])|(?:3[0-1]))\/(\d{4})",
            ErrorMessage = "Birth Date Format Not Correct")]
        public string BirthDate { get; set; } = string.Empty;

        [Required(ErrorMessage = "*Required")]
        [EmailAddress(ErrorMessage = "Incorrect Email Format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "*Required")]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "Passwords Do not Match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Numbers Only!")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Numbers Only!")]
        public string Salary { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Numbers Only!")]
        public string ChargesPerVisit { get; set; } = string.Empty;

        [Range(0, 5, ErrorMessage = "Experience Range should be (0-5)")]
        public string Exp { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Please Select Department")]
        public int Department { get; set; }

        public string Qualification { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
