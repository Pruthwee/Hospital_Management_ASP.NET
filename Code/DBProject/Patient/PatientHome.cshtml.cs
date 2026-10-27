using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using DBProject.DAL;

namespace DBProject.Patient
{
    /// <summary>
    /// Razor Page model for PatientHome — replaces ASP.NET Web Forms PatientHome.aspx / PatientHome.aspx.cs.
    /// Eliminates ViewState, postbacks, and server affinity; enables stateless horizontal scaling on Azure Container Apps.
    ///
    /// Rule: cr-dotnet-0045 — Session State Provider
    /// Remediation: Replaced InProc HttpSessionState with Azure Cache for Redis via IDistributedCache.
    /// Session value (idoriginal) is stored in and retrieved from Redis using RedisSessionHelper,
    /// enabling distributed session state that scales across multiple container instances without server affinity.
    /// </summary>
    public class PatientHomeModel : PageModel
    {
        private readonly IDistributedCache _cache;

        public PatientHomeModel(IDistributedCache cache)
        {
            _cache = cache;
        }

        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string BirthDate { get; set; } = string.Empty;
        public int Age { get; set; } = 0;
        public string Gender { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            LoadPatientInfo();
        }

        private void LoadPatientInfo()
        {
            var objmyDAl = new myDAL();

            // cr-dotnet-0045: Replaced HttpContext.Session.GetInt32 (InProc) with Redis-backed IDistributedCache
            int pid = RedisSessionHelper.GetInt32(_cache, "idoriginal") ?? 0;

            string name = string.Empty;
            string phone = string.Empty;
            string address = string.Empty;
            string birthDate = string.Empty;
            int age = 0;
            string gender = string.Empty;

            int status = objmyDAl.patientInfoDisplayer(pid, ref name, ref phone, ref address, ref birthDate, ref age, ref gender);

            if (status == -1)
            {
                ErrorMessage = "There was some error in retrieving the Patient's Info.";
            }
            else if (status == 0)
            {
                Name = name;
                Phone = phone;
                BirthDate = birthDate;
                Age = age;
                Address = address;
                Gender = gender;
            }
        }
    }
}
