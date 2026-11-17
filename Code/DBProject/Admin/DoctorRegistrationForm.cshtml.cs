using System;
using System.ComponentModel.DataAnnotations;
using DBProject.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DBProject.Admin
{
	// Razor Page replacement of the Web Forms page DoctorRegistrationForm.aspx (same address: /Admin/DoctorRegistrationForm.aspx)
	public class DoctorRegistrationFormModel : PageModel
	{
		[BindProperty]
		public DoctorInput Input { get; set; } = new DoctorInput();

		// Text of the Msg label of the Web Forms page (shown only after a registration)
		public string Message { get; set; }

		public void OnGet()
		{
		}

		public void OnPost()
		{
			// Replaces if (Page.IsValid): the validator controls are DataAnnotations on DoctorInput
			if (!ModelState.IsValid)
			{
				return;
			}

			int exp, salary, chargesPerVisit;
			if (!int.TryParse(Input.Exp, out exp))
			{
				ModelState.AddModelError("Input.Exp", "Value is too large");
			}
			if (!int.TryParse(Input.Salary, out salary))
			{
				ModelState.AddModelError("Input.Salary", "Value is too large");
			}
			if (!int.TryParse(Input.Charges_per_visit, out chargesPerVisit))
			{
				ModelState.AddModelError("Input.Charges_per_visit", "Value is too large");
			}
			if (!ModelState.IsValid)
			{
				return;
			}

			myDAL objmyDAL = new myDAL();

			// Empty optional fields are passed as "" (a TextBox gave "", model binding gives null)
			objmyDAL.AddDoctor(Input.Name, Input.Email, Input.Password, Input.BirthDate, Input.Department, Input.Phone ?? "", Input.Gender[0], Input.Address ?? "", exp, salary, chargesPerVisit, Input.Spec ?? "", Input.Qualification ?? "");

			Message = "doctor Added Succesfully";

			// flushInformation(): clear the fields, the gender radio button keeps its value
			string gender = Input.Gender;
			ModelState.Clear();
			Input = new DoctorInput { Gender = gender };
		}
	}

	// Replaces the CustomValidator DoctorValidate / ValidateDoctorEmail:
	// the e-mail address must not belong to a doctor who is already registered
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class DoctorEmailNotRegisteredAttribute : ValidationAttribute
	{
		public DoctorEmailNotRegisteredAttribute()
			: base("This Email Already exist , kindly choose a different one !")
		{
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			string email = value as string;

			// Like the CustomValidator, an empty value is left to the Required validation
			if (string.IsNullOrEmpty(email))
			{
				return ValidationResult.Success;
			}

			myDAL objmyDAL = new myDAL();

			if (objmyDAL.DoctorEmailAlreadyExist(email) == 1)
			{
				return new ValidationResult(ErrorMessage);
			}

			return ValidationResult.Success;
		}
	}

	// Replaces the RangeValidator of Type="Integer" for a text value: an empty value is left to the
	// Required validation, a value that is not an integer or is out of range is invalid
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class IntegerRangeAttribute : ValidationAttribute
	{
		private readonly int _minimum;
		private readonly int _maximum;

		public IntegerRangeAttribute(int minimum, int maximum)
		{
			_minimum = minimum;
			_maximum = maximum;
		}

		public override bool IsValid(object value)
		{
			string text = value as string;

			if (string.IsNullOrEmpty(text))
			{
				return true;
			}

			int number;
			return int.TryParse(text, out number) && number >= _minimum && number <= _maximum;
		}
	}

	public class DoctorInput
	{
		[Required(ErrorMessage = "* Required")]
		public string Name { get; set; }

		[Required(ErrorMessage = "*Required")]
		[RegularExpression(@"((?:0[1-9])|(?:1[0-2]))\/((?:0[0-9])|(?:[1-2][0-9])|(?:3[0-1]))\/(\d{4})", ErrorMessage = "Birth Date Format Not Correct")]
		public string BirthDate { get; set; }

		[Required(ErrorMessage = "*Required")]
		[RegularExpression(@"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])", ErrorMessage = "Incorrect Email Format")]
		[DoctorEmailNotRegistered]
		public string Email { get; set; }

		[Required(ErrorMessage = "*Required")]
		public string Password { get; set; }

		[Compare("Password", ErrorMessage = "Passwords Do not Match ")]
		public string CPassword { get; set; }

		[RegularExpression(@"^[0-9]+$", ErrorMessage = "Numbers Only !")]
		public string Phone { get; set; }

		[Required(ErrorMessage = "*Required")]
		[RegularExpression(@"^[0-9]+$", ErrorMessage = "Numbers Only !")]
		public string Salary { get; set; }

		[Required(ErrorMessage = "*Required")]
		[RegularExpression(@"^[0-9]+$", ErrorMessage = "Numbers Only !")]
		public string Charges_per_visit { get; set; }

		[Required(ErrorMessage = "*Required")]
		[IntegerRange(0, 5, ErrorMessage = "Experience Range should be (0-5)")]
		public string Exp { get; set; }

		// 0 is the "Select Depatment" entry of the list
		[Range(1, int.MaxValue, ErrorMessage = "Please Select Department")]
		public int Department { get; set; }

		public string Qualification { get; set; }

		public string Spec { get; set; }

		public string Address { get; set; }

		// "M" or "F", posted by the two radio buttons
		[Required]
		[RegularExpression(@"^[MF]$")]
		public string Gender { get; set; } = "M";
	}
}
