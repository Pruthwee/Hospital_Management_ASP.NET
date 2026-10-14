using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject
{
    public partial class AdminHomeModel : PageModel
    {
        public string TotalPatients { get; set; } = "";
        public string Total_Doctors { get; set; } = "";
        public string TotalIncome { get; set; } = "";
        public List<string> AppointmentViewColumns { get; set; } = new List<string>();
        public List<List<string>> AppointmentViewRows { get; set; } = new List<List<string>>();
        public List<string> DepartmentViewColumns { get; set; } = new List<string>();
        public List<List<string>> DepartmentViewRows { get; set; } = new List<List<string>>();

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

            Total_Doctors = arrTable[0].Rows[0][0].ToString();
            TotalPatients = arrTable[1].Rows[0][0].ToString();
            TotalIncome = arrTable[2].Rows[0][0].ToString();

            AppointmentViewColumns = GetColumnNames(arrTable[4]);
            AppointmentViewRows = GetRowData(arrTable[4]);

            DepartmentViewColumns = GetColumnNames(arrTable[3]);
            DepartmentViewRows = GetRowData(arrTable[3]);
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
