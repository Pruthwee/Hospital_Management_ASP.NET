using System;
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace doctor
{
    public partial class doctorhomeModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public doctorhomeModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string Label1 { get; set; } = "";
        public string Label2 { get; set; } = "";
        public string Label3 { get; set; } = "";
        public string Label4 { get; set; } = "";
        public string Label5 { get; set; } = "";
        public string Label6 { get; set; } = "";
        public string Label7 { get; set; } = "";
        public string Label8 { get; set; } = "";
        public string Label9 { get; set; } = "";
        public string Label10 { get; set; } = "";
        public string Label11 { get; set; } = "";
        public string Label12 { get; set; } = "";
        public string Label13 { get; set; } = "";
        public string Label14 { get; set; } = "";

        public void OnGet()
        {
            myDAL objmyDAL = new myDAL();
            DataTable dt = new DataTable();
            int found;

            int did = (int)_httpContextAccessor.HttpContext.Session.GetInt32("idoriginal");

            found = objmyDAL.docinfo_DAL(did, ref dt);

            if (found != 1)
            {
                Label1 = "<script>alert('There was some error');</script>";
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
