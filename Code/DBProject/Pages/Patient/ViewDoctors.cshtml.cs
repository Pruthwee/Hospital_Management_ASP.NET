using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject
{
    public partial class ViewDoctorsModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ViewDoctorsModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string TDoctorText { get; set; } = "";
        public List<string> TDoctorGridColumns { get; set; } = new List<string>();
        public List<List<string>> TDoctorGridRows { get; set; } = new List<List<string>>();

        public void OnGet(int? rowIndex = null)
        {
            _httpContextAccessor.HttpContext.Session.SetString("dID", "");
            deptDoctorInfo();
        }

        protected void deptDoctorInfo()
        {
            myDAL objmyDAl = new myDAL();

            DataTable DT = new DataTable();

            string deptName = _httpContextAccessor.HttpContext.Session.GetString("deptOriginal");

            int status = objmyDAl.getDeptDoctorInfo(deptName, ref DT);

            if (status == -1)
            {
                TDoctorText = "There was some error in retrieving the Doctors Information.";
            }
            else
            {
                TDoctorText = "Following are our Specialized Doctors of " + deptName + " Department:";
                TDoctorGridColumns = GetColumnNames(DT);
                TDoctorGridRows = GetRowData(DT);
            }
        }

        public IActionResult OnPostSelect(int rowIndex)
        {
            string dID = TDoctorGridRows[rowIndex][2];
            _httpContextAccessor.HttpContext.Session.SetString("dID", dID);
            return Redirect("~/Pages/Patient/DoctorProfile.cshtml");
        }

        private List<string> GetColumnNames(DataTable table)
        {
            List<string> columns = new List<string>();
            foreach (DataColumn col in table.Columns)
            {
                columns.Add(col.ColumnName);
            }
            return columns;
        }

        private List<List<string>> GetRowData(DataTable table)
        {
            List<List<string>> rows = new List<List<string>>();
            foreach (DataRow row in table.Rows)
            {
                List<string> rowData = new List<string>();
                foreach (var item in row.ItemArray)
                {
                    rowData.Add(item.ToString());
                }
                rows.Add(rowData);
            }
            return rows;
        }
    }
}
