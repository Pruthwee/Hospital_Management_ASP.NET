using System;
using System.Data;
using DBProject;
using DBProject.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace doctor
{
    public class doctorhomeModel : PageModel
    {
        public string Label1 = "";
        public string Label2 = "";
        public string Label3 = "";
        public string Label4 = "";
        public string Label5 = "";
        public string Label6 = "";
        public string Label7 = "";
        public string Label8 = "";
        public string Label9 = "";
        public string Label10 = "";
        public string Label11 = "";
        public string Label12 = "";
        public string Label13 = "";
        public string Label14 = "";

        public void OnGet()
        {
            myDAL objmyDAL = new myDAL();
            DataTable dt = new DataTable();
            int found;
            int did = HttpContext.Session.GetRequiredInt32("idoriginal");

            found = objmyDAL.docinfo_DAL(did, ref dt);


            if (found != 1)
            {
                { ViewData["Alert"] = "There was some error"; }
            }
            else
            {
                Label1 = dt.Rows[0][1].ToString();
                Label2 = dt.Rows[0][2].ToString();
                Label3 = dt.Rows[0][3].ToString();
                Label4 = dt.Rows[0][4].ToString();
                Label5 = dt.Rows[0][5].ToString();
                Label6 = dt.Rows[0][6].ToString();
                Label7 = dt.Rows[0][7].ToString();
                Label8 = dt.Rows[0][8].ToString();
                Label9 = dt.Rows[0][9].ToString();
                Label10 = dt.Rows[0][10].ToString();
                Label11 = dt.Rows[0][11].ToString();
                Label12 = dt.Rows[0][12].ToString();
                Label13 = dt.Rows[0][13].ToString();
                Label14 = dt.Rows[0][14].ToString();

            }
        }
    }
}
