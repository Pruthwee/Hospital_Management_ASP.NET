using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject
{
    public partial class BillsHistoryModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BillsHistoryModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string BHistoryText { get; set; } = "";
        public List<string> BHistoryGridColumns { get; set; } = new List<string>();
        public List<List<string>> BHistoryGridRows { get; set; } = new List<List<string>>();

        public void OnGet()
        {
            billHistory();
        }

        protected void billHistory()
        {
            myDAL objmyDAl = new myDAL();

            DataTable DT = new DataTable();

            int id = (int)_httpContextAccessor.HttpContext.Session.GetInt32("idoriginal");

            int status = objmyDAl.getBillHistory(id, ref DT);

            if (status == -1)
            {
                BHistoryText = "There was some error in retrieving the Patient's Bill History.";
            }
            else if (status == 0)
            {
                BHistoryText = "There is currently no bill history of yours.";
            }
            else
            {
                BHistoryText = status + " Bill(s) are found: ";
                BHistoryGridColumns = GetColumnNames(DT);
                BHistoryGridRows = GetRowData(DT);
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
