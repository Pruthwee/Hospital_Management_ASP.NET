using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject
{
    public partial class ManageClinicModel : PageModel
    {
        public string Category { get; set; } = "DOCTOR";
        public string SearchText { get; set; } = "";
        public string Msg { get; set; } = "";
        public string MyDivHtml { get; set; } = "";
        public List<string> ManageColumns { get; set; } = new List<string>();
        public List<List<string>> ManageRows { get; set; } = new List<List<string>>();

        public void OnGet(string Category = null, string txtSearch = null, string select = null, string delete = null, string rowIndex = null)
        {
            if (Category != null)
            {
                this.Category = Category;
            }
            else
            {
                this.Category = "DOCTOR";
            }

            if (txtSearch != null)
            {
                SearchText = txtSearch;
            }

            if (select != null)
            {
                int idx = Convert.ToInt32(rowIndex);
                SelectCommand(idx);
            }

            if (delete != null)
            {
                int idx = Convert.ToInt32(rowIndex);
                DeleteDoctor_Click(idx);
            }

            LoadGrid(SearchText, Category ?? this.Category);
        }

        public IActionResult OnPost(string Category = null, string txtSearch = null, string search = null)
        {
            if (Category != null)
            {
                this.Category = Category;
            }

            if (txtSearch != null)
            {
                SearchText = txtSearch;
            }

            LoadGrid(SearchText, this.Category);
            return Page();
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
                    ManageColumns = GetColumnNames(table);
                    ManageRows = GetRowData(table);
                }
                else
                {
                    Msg = "No Doctors to show";
                }
            }
            else if (Category == "PATIENT")
            {
                objmyDaL.LoadPatient(ref table, SearchQuery);

                if (table != null && table.Rows.Count > 0)
                {
                    ManageColumns = GetColumnNames(table);
                    ManageRows = GetRowData(table);
                }
                else
                {
                    Msg = "No Pateints to show";
                }
            }
            else
            {
                objmyDaL.LoadOtherStaff(ref table, SearchQuery);

                if (table != null && table.Rows.Count > 0)
                {
                    ManageColumns = GetColumnNames(table);
                    ManageRows = GetRowData(table);
                }
                else
                {
                    Msg = "No Staff Member to show";
                }
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

        /*EVENT HANDLER FOR DELETE BUTTON IN GRID*/
        protected void DeleteDoctor_Click(int rowIndex)
        {
            myDAL objDAL = new myDAL();

            string id = ManageRows[rowIndex][1];

            if (Category == "DOCTOR")
            {
                if (objDAL.DeleteDoctor(Convert.ToInt32(id)) == 1)
                {
                    Msg = " Doctor No: " + id + " Deleted";
                    LoadGrid(SearchText, Category);
                }
                else
                {
                    Msg = "there was some error";
                }
            }
            else if (Category == "PATIENT")
            {
                Msg = "You are not Authorized to Delete a Patient";
            }
            else
            {
                if (objDAL.DeleteStaff(Convert.ToInt32(id)) == 1)
                {
                    Msg = "Staff No: " + id + " Deleted ";
                    LoadGrid(SearchText, Category);
                }
                else
                {
                    Msg = "There was some Error";
                }
            }
        }

        protected void SelectCommand(int rowIndex)
        {
            string id = ManageRows[rowIndex][1];
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

            if (Category == "DOCTOR")
            {
                if (objDAL.GET_DOCTOR_PROFILE(Convert.ToInt32(id), ref name, ref phone, ref gender, ref charges_Per_Visit, ref ReputeIndex, ref PatientsTreated, ref qualification, ref specialization, ref workE, ref age) == 1)
                {
                    MyDivHtml = "<p><b>Name:</b></p>" + name +
                                " <p><b>phone:</b></p>" + phone +
                                "<p><b>gender:</b></p>" + gender +
                                "<p><b>Qualification:</b></p>" + qualification +
                                "<p><b> Age:</b></p>" + age +
                                "<p><b>Charges:</b></p> " + charges_Per_Visit +
                                "<p><b>Repute index:</b></p>" + ReputeIndex;
                }
                else
                {
                    Msg = "there was some error";
                }
            }
            else if (Category == "PATIENT")
            {
                if (objDAL.GETPATIENT(Convert.ToInt32(id), ref name, ref phone, ref address, ref bDate, ref age, ref gender) == 0)
                {
                    MyDivHtml = "<p><b>Name:</b></p>" + name +
                                " <p><b>phone:</b></p>" + phone +
                                "<p><b>gender:</b></p>" + gender +
                                "<p><b>Address:</b></p>" + address +
                                "<p><b> Age:</b></p>" + age;
                }
                else
                {
                    Msg = "there was some error";
                }
            }
            else
            {
                string designation = "";
                int s = 0;

                if (objDAL.GETSATFF(Convert.ToInt32(id), ref name, ref phone, ref address, ref gender, ref designation, ref s) == 1)
                {
                    MyDivHtml = "<p><b>Name:</b></p>" + name +
                                " <p><b>phone:</b></p>" + phone +
                                "<p><b>gender:</b></p>" + gender +
                                "<p><b>Address:</b></p>" + address +
                                "<p><b> salary:</b></p>" + s;
                }
                else
                {
                    Msg = "there was some error";
                }
            }
        }
    }
}
