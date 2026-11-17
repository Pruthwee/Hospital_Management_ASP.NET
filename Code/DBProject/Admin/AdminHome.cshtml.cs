using System;
using System.Data;
using DBProject.DAL;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DBProject.Admin
{
	// Razor Page replacement of the Web Forms page AdminHome.aspx (same address: /Admin/AdminHome.aspx)
	public class AdminHomeModel : PageModel
	{
		// Values of the three Label controls
		public string TotalPatients { get; set; }
		public string TotalDoctors { get; set; }
		public string TotalIncome { get; set; }

		// Data of the two GridView controls
		public DataTable AppointmentView { get; set; }
		public DataTable DepartmentView { get; set; }

		// Page_Load of the Web Forms page: loads the clinic statistics
		public void OnGet()
		{
			GetAdminHomeInformation();
		}

		public void GetAdminHomeInformation()
		{
			myDAL objmyDAL = new myDAL();

			DataTable[] arrTable = new DataTable[5];
			for (int i = 0; i < 5; i++)
			{
				arrTable[i] = new DataTable();
			}


			objmyDAL.GetAdminHomeInformation(ref arrTable);

			// arrTable[0] is Total_Patient, arrTable[1] is Total_Doctors, arrTable[2] is Income
			TotalPatients = arrTable[0].Rows[0][0].ToString();
			TotalDoctors = arrTable[1].Rows[0][0].ToString();
			TotalIncome = arrTable[2].Rows[0][0].ToString();

			DepartmentView = arrTable[3];
			AppointmentView = arrTable[4];
		}
	}
}
