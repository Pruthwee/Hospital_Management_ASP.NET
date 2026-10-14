using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace doctor
{
    public partial class pendingappointmentModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public pendingappointmentModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public List<string> PendingColumns { get; set; } = new List<string>();
        public List<List<string>> PendingRows { get; set; } = new List<List<string>>();

        public void OnGet(string approve = null, string delete = null, int? rowIndex = null)
        {
            if (approve != null)
            {
                update_appointment((int)rowIndex);
            }

            if (delete != null)
            {
                Delete_appointment((int)rowIndex);
            }

            loadgrid();
        }

        public void loadgrid()
        {
            myDAL objDAL = new myDAL();

            int did = (int)_httpContextAccessor.HttpContext.Session.GetInt32("idoriginal");

            DataTable DT = new DataTable();

            objDAL.GetAllpendingappointments_DAL(did, ref DT);

            PendingColumns = GetColumnNames(DT);
            PendingRows = GetRowData(DT);
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

        protected void update_appointment(int rowIndex)
        {
            string appointmentidStr = PendingRows[rowIndex][1];
            int appointmentid = Convert.ToInt32(appointmentidStr);

            myDAL objmyDAL = new myDAL();
            int result = objmyDAL.UpdateAppointment_DAL(appointmentid);
            loadgrid();
        }

        protected void Delete_appointment(int rowIndex)
        {
            string appointmentidStr = PendingRows[rowIndex][1];
            int appointmentid = Convert.ToInt32(appointmentidStr);

            myDAL objDAL = new myDAL();

            if (objDAL.Deleteappointment_DAL(appointmentid) == 1)
            {
                loadgrid();
            }
        }
    }
}
