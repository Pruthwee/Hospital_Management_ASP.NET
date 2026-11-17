using System;
using System.ComponentModel.DataAnnotations;
using DBProject.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DBProject.Admin
{
	// Razor Page replacement of the Web Forms page AddStaff.aspx (same address: /Admin/AddStaff.aspx)
	public class AddStaffModel : PageModel
	{
		[BindProperty]
		public StaffInput Input { get; set; } = new StaffInput();

		// Text of the Msg label of the Web Forms page (shown only after a registration)
		public string Message { get; set; }

		public void OnGet()
		{
		}

		public void OnPost()
		{
			// Replaces if (Page.IsValid): the validator controls are DataAnnotations on StaffInput
			if (!ModelState.IsValid)
			{
				return;
			}

			int salary;
			if (!int.TryParse(Input.Salary, out salary))
			{
				ModelState.AddModelError("Input.Salary", "Value is too large");
				return;
			}

			myDAL objmyDAL = new myDAL();

			// As in the Web Forms page the result of AddStaff does not change what happens next:
			// the original "if (... == 1) ;" ended in an empty statement, so the message was always shown.
			// Empty optional fields are passed as "" (a TextBox gave "", model binding gives null)
			objmyDAL.AddStaff(Input.Name, Input.BirthDate, Input.Phone ?? "", Input.Gender[0], Input.Address ?? "", salary, Input.Qual ?? "", Input.Designation);

			Message = Input.Designation + " Added Succesfully";

			// flushInformation(): clear the fields, the gender radio button keeps its value
			string gender = Input.Gender;
			ModelState.Clear();
			Input = new StaffInput { Gender = gender };
		}
	}

	public class StaffInput
	{
		[Required(ErrorMessage = "*Required")]
		public string Name { get; set; }

		[Required(ErrorMessage = "*Required")]
		[RegularExpression(@"((?:0[1-9])|(?:1[0-2]))\/((?:0[0-9])|(?:[1-2][0-9])|(?:3[0-1]))\/(\d{4})", ErrorMessage = "Birth Date Format Not Correct")]
		public string BirthDate { get; set; }

		[RegularExpression(@"^[0-9]+$", ErrorMessage = "Numbers Only !")]
		public string Phone { get; set; }

		[Required(ErrorMessage = "*Required")]
		[RegularExpression(@"^[0-9]+$", ErrorMessage = "Numbers Only !")]
		public string Salary { get; set; }

		public string Qual { get; set; }

		[Required(ErrorMessage = "*Required")]
		public string Designation { get; set; }

		public string Address { get; set; }

		// "M" or "F", posted by the two radio buttons
		[Required]
		[RegularExpression(@"^[MF]$")]
		public string Gender { get; set; } = "M";
	}
}
