using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using DBProject.DAL;

namespace DBProject.Patient
{
    /// <summary>
    /// Razor Page model for DoctorProfile — replaces ASP.NET Web Forms DoctorProfile.aspx / DoctorProfile.aspx.cs.
    /// Eliminates ViewState, postbacks, and server affinity; enables stateless horizontal scaling on Azure Container Apps.
    ///
    /// Rule: cr-dotnet-0045 — Session State Provider
    /// Remediation: Replaced InProc HttpSessionState with Azure Cache for Redis via IDistributedCache.
    /// Session values (dID, deptOriginal) are stored in and retrieved from Redis using RedisSessionHelper,
    /// enabling distributed session state that scales across multiple container instances without server affinity.
    /// </summary>
    public class DoctorProfileModel : PageModel
    {
        private readonly IDistributedCache _cache;

        public DoctorProfileModel(IDistributedCache cache)
        {
            _cache = cache;
        }

        public string DName { get; set; } = string.Empty;
        public string DPhone { get; set; } = string.Empty;
        public string DQualification { get; set; } = string.Empty;
        public string DSpecialization { get; set; } = string.Empty;
        public string DWork { get; set; } = string.Empty;
        public string DAge { get; set; } = string.Empty;
        public string DGender { get; set; } = string.Empty;
        public string DDept { get; set; } = string.Empty;
        public string DCharges { get; set; } = string.Empty;
        public string DRI { get; set; } = string.Empty;
        public string DPT { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            LoadDoctorInfo();
        }

        private void LoadDoctorInfo()
        {
            var objmyDAl = new myDAL();

            // cr-dotnet-0045: Replaced HttpContext.Session.GetString (InProc) with Redis-backed IDistributedCache
            string? dID1 = RedisSessionHelper.GetString(_cache, "dID");
            int dID = Convert.ToInt32(dID1);

            string name = string.Empty;
            string phone = string.Empty;
            string gender = string.Empty;
            float charges_Per_Visit = 0;
            float reputeIndex = 0;
            int patientsTreated = 0;
            string qualification = string.Empty;
            string specialization = string.Empty;
            int workE = 0;
            int age = 0;

            // cr-dotnet-0045: Replaced HttpContext.Session.GetString (InProc) with Redis-backed IDistributedCache
            string deptName = RedisSessionHelper.GetString(_cache, "deptOriginal") ?? string.Empty;

            int status = objmyDAl.doctorInfoDisplayer(dID, ref name, ref phone, ref gender, ref charges_Per_Visit, ref reputeIndex, ref patientsTreated, ref qualification, ref specialization, ref workE, ref age);

            if (status == -1)
            {
                ErrorMessage = "There was some error in retrieving the Doctor's Info.";
            }
            else
            {
                DName = name;
                DPhone = phone;
                DQualification = qualification;
                DSpecialization = specialization;
                DWork = workE.ToString();
                DAge = age.ToString();
                DGender = gender;
                DDept = deptName;
                DCharges = charges_Per_Visit.ToString();
                DRI = reputeIndex.ToString();
                DPT = patientsTreated.ToString();
            }
        }
    }
}
