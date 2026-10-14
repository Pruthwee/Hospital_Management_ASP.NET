using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace doctor
{
    public partial class patienthistoryModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public patienthistoryModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public List<string> PatientsGridColumns { get; set; } = new List<string>();
        public List<List<string>> PatientsGridRows { get; set; } = new List<List<string>>();

        public void OnGet(int? rowIndex = null)
        {
            loadgrid();
        }

        public IActionResult OnPost(int rowIndex)
        {
            _httpContextAccessor.HttpContext.Session.SetInt32("appointid", rowIndex);
            return Redirect("~/Pages/Doctor/HistoryUpdate.cshtml");
        }

        public void loadgrid()
        {
            myDAL objDAL = new myDAL();

            int did = (int)_httpContextAccessor.HttpContext.Session.GetInt32("idoriginal");

            DataTable DT = new DataTable();

            objDAL.GetAllpendingappointments_DAL(did, ref DT);

            PatientsGridColumns = GetColumnNames(DT);
            PatientsGridRows = GetRowData(DT);
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
