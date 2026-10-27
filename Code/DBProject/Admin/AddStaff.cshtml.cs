using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using DBProject.DAL;

namespace DBProject.Admin
{
    /// <summary>
    /// Razor Page model for Staff Registration — replaces ASP.NET Web Forms AddStaff.aspx / AddStaff.aspx.cs.
    /// Eliminates ViewState, postbacks, and server affinity; enables stateless horizontal scaling on Azure Container Apps.
    /// </summary>
    public class AddStaffModel : PageModel
    {
        [BindProperty]
        public AddStaffInputModel Input { get; set; } = new();

        public string Message { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string gender = Request.Form["Gender"].ToString();
            if (string.IsNullOrEmpty(gender))
            {
                gender = "M";
            }

            var objmyDAL = new myDAL();
            int salary = int.Parse(Input.Salary);

            int result = objmyDAL.AddStaff(
                Input.Name,
                Input.BirthDate,
                Input.Phone,
                gender[0],
                Input.Address,
                salary,
                Input.Qual,
                Input.Designation);

            if (result == 1)
            {
                Message = Input.Designation + " Added Successfully";
                Input = new AddStaffInputModel();
            }

            return Page();
        }
    }

    public class AddStaffInputModel
    {
        [Required(ErrorMessage = "*Required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "*Required")]
        [RegularExpression(@"((?:0[1-9])|(?:1[0-2]))\/((?:0[0-9])|(?:[1-2][0-9])|(?:3[0-1]))\/(\d{4})",
            ErrorMessage = "Birth Date Format Not Correct")]
        public string BirthDate { get; set; } = string.Empty;

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Numbers Only!")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "*Required")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Numbers Only!")]
        public string Salary { get; set; } = string.Empty;

        public string Qual { get; set; } = string.Empty;

        [Required(ErrorMessage = "*Required")]
        public string Designation { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
    }
}
