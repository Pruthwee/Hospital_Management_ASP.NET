using System;
using System.Collections.Generic;
using System.Data;
using DBProject.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DBProject.Admin
{
	// Razor Page replacement of the Web Forms page ManageClinic.aspx (same address: /Admin/ManageClinic.aspx)
	public class ManageClinicModel : PageModel
	{
		// Value of the checked radio button of the group "Catagory": "D" = Doctor, "P" = Patient, "O" = Other Staff
		[BindProperty]
		public string Category { get; set; } = "D";

		// Text of the txtSearch TextBox
		[BindProperty]
		public string SearchText { get; set; }

		// Text of the Msg label
		public string Message { get; set; }

		// Data and caption of the Manage GridView (the table is empty when there is nothing to show)
		public DataTable Table { get; set; }
		public string Caption { get; set; } = "Doctors Table";

		// Content of the mydiv panel: label / value pairs of the selected row
		public List<KeyValuePair<string, string>> Details { get; set; }

		public bool IsDoctor
		{
			get { return Category == "D"; }
		}

		public bool IsPatient
		{
			get { return Category == "P"; }
		}

		public bool IsOtherStaff
		{
			get { return !IsDoctor && !IsPatient; }
		}

		// Page_Load with !IsPostBack
		public void OnGet()
		{
			LoadGrid("", "DOCTOR");
		}

		/*EVENT HANDLER FOR ANY CHANGE IN THE RADIO BUTTON*/
		public void OnPostChangeCategory()
		{
			LoadGrid("", CurrentCategoryName());
		}

		/*THIS FUNCTION WILL SEARCH THE NAME AND GIVE RESULTS OR RETURN ALL TUPLES FROM DATABASE IN THE GRID VIE*/
		public void OnPostSearch()
		{
			LoadGrid(SearchText ?? "", CurrentCategoryName());
		}

		/*EVENT HANDLER FOR DELETE BUTTON IN GRID*/
		public void OnPostDelete(int id)
		{
			myDAL objDAL = new myDAL();

			if (IsDoctor)
			{
				if (objDAL.DeleteDoctor(id) == 1)
				{
					Message = " Doctor No: " + id + " Deleted";
					LoadGrid("", "DOCTOR");
				}
				else
				{
					LoadGrid("", "DOCTOR");
					Message = "there was some error";
				}
			}
			else if (IsPatient)
			{
				LoadGrid("", "PATIENT");
				Message = "You are not Authorized to Delete a Patient";
			}
			else
			{
				if (objDAL.DeleteStaff(id) == 1)
				{
					Message = "Staff No: " + id + " Deleted ";
					LoadGrid("", "OTHERSTAFF");
				}
				else
				{
					LoadGrid("", "OTHERSTAFF");
					Message = "There was some Error";
				}
			}
		}

		/*EVENT HANDLER FOR THE SELECT BUTTON IN GRID*/
		public void OnPostSelect(int id)
		{
			LoadGrid("", CurrentCategoryName());

			myDAL objDAL = new myDAL();

			string name = "";
			string phone = "";
			string gender = "";
			string address = "";
			string bDate = "";

			float charges_Per_Visit = 0;
			float ReputeIndex = 0;
			int PatientsTreated = 0;
			string qualification = "";
			string specialization = "";
			int workE = 0;
			int age = 0;

			if (IsDoctor)
			{
				if (objDAL.GET_DOCTOR_PROFILE(id, ref name, ref phone, ref gender, ref charges_Per_Visit, ref ReputeIndex, ref PatientsTreated, ref qualification, ref specialization, ref workE, ref age) == 1)
				{
					Details = new List<KeyValuePair<string, string>>
					{
						Detail("Name:", name),
						Detail("phone:", phone),
						Detail("gender:", gender),
						Detail("Qualification:", qualification),
						Detail(" Age:", age.ToString()),
						Detail("Charges:", charges_Per_Visit.ToString()),
						Detail("Repute index:", ReputeIndex.ToString())
					};
				}
				else
				{
					Message = "there was some error";
				}
			}
			else if (IsPatient)
			{
				if (objDAL.GETPATIENT(id, ref name, ref phone, ref address, ref bDate, ref age, ref gender) == 0)
				{
					Details = new List<KeyValuePair<string, string>>
					{
						Detail("Name:", name),
						Detail("phone:", phone),
						Detail("gender:", gender),
						Detail("Address:", address),
						Detail(" Age:", age.ToString())
					};
				}
				else
				{
					Message = "there was some error";
				}
			}
			else
			{
				string designation = "";
				int s = 0;

				if (objDAL.GETSATFF(id, ref name, ref phone, ref address, ref gender, ref designation, ref s) == 1)
				{
					Details = new List<KeyValuePair<string, string>>
					{
						Detail("Name:", name),
						Detail("phone:", phone),
						Detail("gender:", gender),
						Detail("Address:", address),
						Detail(" salary:", s.ToString())
					};
				}
				else
				{
					Message = "there was some error";
				}
			}
		}

		private static KeyValuePair<string, string> Detail(string label, string value)
		{
			return new KeyValuePair<string, string>(label, value);
		}

		private string CurrentCategoryName()
		{
			if (IsDoctor)
			{
				return "DOCTOR";
			}
			else if (IsPatient)
			{
				return "PATIENT";
			}
			return "OTHERSTAFF";
		}

		/*THIS FUNTION WILL LOAD THE INFORMATION OF ALL DOCTORS AND BIND THEM TO THE GRID*/
		protected void LoadGrid(string SearchQuery, string Category)
		{
			myDAL objmyDaL = new myDAL();
			DataTable table = new DataTable();


			if (Category == "DOCTOR")
			{
				objmyDaL.LoadDoctor(ref table, SearchQuery);

				if (table != null && table.Rows.Count > 0)
				{
					Table = table;
				}
				else
				{
					Message = "No Doctors to show";
				}
			}
			else if (Category == "PATIENT")
			{
				objmyDaL.LoadPatient(ref table, SearchQuery);

				if (table != null && table.Rows.Count > 0)
				{
					Table = table;
				}
				else
				{
					Message = "No Pateints to show";
				}
			}
			else
			{
				objmyDaL.LoadOtherStaff(ref table, SearchQuery);

				if (table != null && table.Rows.Count > 0)
				{
					Table = table;
					Caption = "Other Staff Table:";
				}
				else
				{
					Message = "No Staff Member to show";
				}
			}
		}
	}
}
