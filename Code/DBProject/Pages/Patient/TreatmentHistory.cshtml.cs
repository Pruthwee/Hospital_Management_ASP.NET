using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject
{
    public partial class TreatmentHistoryModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TreatmentHistoryModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string THistoryText { get; set; } = "";
        public List<string> THistoryGridColumns { get; set; } = new List<string>();
        public List<List<string>> THistoryGridRows { get; set; } = new List<List<string>>();

        public void OnGet()
        {
            treatmentHistory();
        }

        protected void treatmentHistory()
        {
            myDAL objmyDAl = new myDAL();

            DataTable DT = new DataTable();

            int id = (int)_httpContextAccessor.HttpContext.Session.GetInt32("idoriginal");

            int status = objmyDAl.getTreatmentHistory(id, ref DT);

            if (status == -1)
            {
                THistoryText = "There was some error in retrieving the Patient's Treatment History.";
            }
            else if (status == 0)
            {
                THistoryText = "There is currently no treatment history of yours.";
            }
            else
            {
                THistoryText = "Treatment History of " + status + " Appointment(s) is found: ";
                THistoryGridColumns = GetColumnNames(DT);
                THistoryGridRows = GetRowData(DT);
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
