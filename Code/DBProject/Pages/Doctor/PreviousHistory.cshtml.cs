using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject.Doctor
{
    public partial class PreviousHistoryModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PreviousHistoryModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string PHistoryText { get; set; } = "";
        public List<string> PHistoryGridColumns { get; set; } = new List<string>();
        public List<List<string>> PHistoryGridRows { get; set; } = new List<List<string>>();

        public void OnGet()
        {
            PatHistory();
        }

        protected void PatHistory()
        {
            myDAL objmyDAl = new myDAL();

            DataTable DT = new DataTable();

            int id = (int)_httpContextAccessor.HttpContext.Session.GetInt32("idoriginal");

            int status = objmyDAl.getPHistory(id, ref DT);

            if (status == -1)
            {
                PHistoryText = "There was some error in retrieving the Patients History.";
            }
            else
            {
                PHistoryGridColumns = GetColumnNames(DT);
                PHistoryGridRows = GetRowData(DT);
            }
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
