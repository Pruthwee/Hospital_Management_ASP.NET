using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject
{
    public partial class AppointmentTakerModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppointmentTakerModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string PAppointmentText { get; set; } = "";
        public List<string> PAppointmentGridColumns { get; set; } = new List<string>();
        public List<List<string>> PAppointmentGridRows { get; set; } = new List<List<string>>();

        public IActionResult OnGet(string selectSlot = null)
        {
            _httpContextAccessor.HttpContext.Session.SetString("freeSlot", "");

            if (selectSlot != null)
            {
                freeSlots();
                int idx = Convert.ToInt32(selectSlot);
                string appointment = PAppointmentGridRows[idx][2];
                string[] tokens = appointment.Split(':');
                _httpContextAccessor.HttpContext.Session.SetString("freeSlot", tokens[0]);
                return Redirect("~/Pages/Patient/AppointmentRequestSent.cshtml");
            }

            freeSlots();
            return Page();
        }

        protected void freeSlots()
        {
            myDAL objmyDAl = new myDAL();

            DataTable DT = new DataTable();

            string dID1 = _httpContextAccessor.HttpContext.Session.GetString("dID");

            int dID = Convert.ToInt32(dID1);

            int pID = (int)_httpContextAccessor.HttpContext.Session.GetInt32("idoriginal");

            int status = objmyDAl.getFreeSlots(dID, pID, ref DT);

            if (status == -1)
            {
                PAppointmentText = "There was some error in retrieving the Doctors's Free Slots.";
            }
            else if (status == 0)
            {
                PAppointmentText = "There is currently no free slot of this doctor.";
            }
            else if (status > 0)
            {
                PAppointmentText = "The following are the " + status + " free slots of this doctor for today :";
                PAppointmentGridColumns = GetColumnNames(DT);
                PAppointmentGridRows = GetRowData(DT);
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
