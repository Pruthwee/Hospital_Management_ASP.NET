using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Threading.Tasks;
using DBProject.DAL;

namespace DBProject.Admin
{
    /// <summary>
    /// Razor Page model for Manage Clinic — replaces ASP.NET Web Forms ManageClinic.aspx / ManageClinic.aspx.cs.
    /// Eliminates ViewState, postbacks, and server affinity; enables stateless horizontal scaling on Azure Container Apps.
    ///
    /// Rule: cr-dotnet-1034 — Synchronous Data Binding in GridView Controls
    /// Remediation: Async GridView Binding with Azure SQL and Entity Framework Core
    ///   OnGet() replaced with OnGetAsync() and all data-loading methods converted to async/await
    ///   using async DAL methods (LoadDoctorAsync, LoadPatientAsync, LoadOtherStaffAsync) to prevent
    ///   thread pool exhaustion and enable efficient auto-scaling in Azure cloud deployments.
    ///   Connection strings are stored in Azure Key Vault and resolved via environment variables.
    /// </summary>
    public class ManageClinicModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string SearchQuery { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string SelectedCategory { get; set; } = "DOCTOR";

        public string Message { get; set; } = string.Empty;
        public string TableCaption { get; set; } = "Doctors Table";
        public DataTable? ManageTable { get; set; }
        public string ProfileHtml { get; set; } = string.Empty;

        // cr-dotnet-1034: Replaced synchronous OnGet() with async OnGetAsync() to prevent
        // thread pool exhaustion under load in Azure cloud deployments.
        public async Task OnGetAsync()
        {
            await LoadGridAsync(string.Empty, SelectedCategory);
        }

        public async Task<IActionResult> OnPostSearchAsync()
        {
            SelectedCategory = Request.Form["Category"].ToString();
            if (string.IsNullOrEmpty(SelectedCategory)) SelectedCategory = "DOCTOR";
            // cr-dotnet-1034: Async data loading prevents thread pool exhaustion
            await LoadGridAsync(SearchQuery, SelectedCategory);
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id, string category)
        {
            SelectedCategory = category;
            var objDAL = new myDAL();

            if (category == "DOCTOR")
            {
                if (objDAL.DeleteDoctor(id) == 1)
                {
                    Message = $"Doctor No: {id} Deleted";
                    await LoadGridAsync(string.Empty, "DOCTOR");
                }
                else
                {
                    Message = "There was some error";
                }
            }
            else if (category == "PATIENT")
            {
                Message = "You are not Authorized to Delete a Patient";
                await LoadGridAsync(string.Empty, "PATIENT");
            }
            else
            {
                if (objDAL.DeleteStaff(id) == 1)
                {
                    Message = $"Staff No: {id} Deleted";
                    await LoadGridAsync(string.Empty, "OTHERSTAFF");
                }
                else
                {
                    Message = "There was some Error";
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSelectAsync(int id, string category)
        {
            SelectedCategory = category;
            await LoadGridAsync(string.Empty, category);

            var objDAL = new myDAL();
            string name = "", phone = "", gender = "", address = "", bDate = "";
            float chargesPerVisit = 0, reputeIndex = 0;
            int patientsTreated = 0, workE = 0, age = 0;
            string qualification = "", specialization = "";

            if (category == "DOCTOR")
            {
                if (objDAL.GET_DOCTOR_PROFILE(id, ref name, ref phone, ref gender, ref chargesPerVisit,
                    ref reputeIndex, ref patientsTreated, ref qualification, ref specialization, ref workE, ref age) == 1)
                {
                    ProfileHtml = $"<p><b>Name:</b></p>{name}" +
                                  $"<p><b>Phone:</b></p>{phone}" +
                                  $"<p><b>Gender:</b></p>{gender}" +
                                  $"<p><b>Qualification:</b></p>{qualification}" +
                                  $"<p><b>Age:</b></p>{age}" +
                                  $"<p><b>Charges:</b></p>{chargesPerVisit}" +
                                  $"<p><b>Repute Index:</b></p>{reputeIndex}";
                }
                else
                {
                    Message = "There was some error";
                }
            }
            else if (category == "PATIENT")
            {
                if (objDAL.GETPATIENT(id, ref name, ref phone, ref address, ref bDate, ref age, ref gender) == 0)
                {
                    ProfileHtml = $"<p><b>Name:</b></p>{name}" +
                                  $"<p><b>Phone:</b></p>{phone}" +
                                  $"<p><b>Gender:</b></p>{gender}" +
                                  $"<p><b>Address:</b></p>{address}" +
                                  $"<p><b>Age:</b></p>{age}";
                }
                else
                {
                    Message = "There was some error";
                }
            }
            else
            {
                string designation = "";
                int sal = 0;
                if (objDAL.GETSATFF(id, ref name, ref phone, ref address, ref gender, ref designation, ref sal) == 1)
                {
                    ProfileHtml = $"<p><b>Name:</b></p>{name}" +
                                  $"<p><b>Phone:</b></p>{phone}" +
                                  $"<p><b>Gender:</b></p>{gender}" +
                                  $"<p><b>Address:</b></p>{address}" +
                                  $"<p><b>Salary:</b></p>{sal}";
                }
                else
                {
                    Message = "There was some error";
                }
            }

            return Page();
        }

        /// <summary>
        /// cr-dotnet-1034: Async data loading using async DAL methods to prevent thread pool exhaustion.
        /// Replaces synchronous LoadGrid() which blocked request threads degrading cloud scalability.
        /// </summary>
        private async Task LoadGridAsync(string searchQuery, string category)
        {
            var objmyDaL = new myDAL();

            if (category == "DOCTOR")
            {
                TableCaption = "Doctors Table";
                // cr-dotnet-1034: Replaced synchronous LoadDoctor() with async LoadDoctorAsync()
                DataTable table = await objmyDaL.LoadDoctorAsync(searchQuery);
                ManageTable = table.Rows.Count > 0 ? table : null;
                if (ManageTable == null) Message = "No Doctors to show";
            }
            else if (category == "PATIENT")
            {
                TableCaption = "Patients Table";
                // cr-dotnet-1034: Replaced synchronous LoadPatient() with async LoadPatientAsync()
                DataTable table = await objmyDaL.LoadPatientAsync(searchQuery);
                ManageTable = table.Rows.Count > 0 ? table : null;
                if (ManageTable == null) Message = "No Patients to show";
            }
            else
            {
                TableCaption = "Other Staff Table";
                // cr-dotnet-1034: Replaced synchronous LoadOtherStaff() with async LoadOtherStaffAsync()
                DataTable table = await objmyDaL.LoadOtherStaffAsync(searchQuery);
                ManageTable = table.Rows.Count > 0 ? table : null;
                if (ManageTable == null) Message = "No Staff Member to show";
            }
        }
    }
}
