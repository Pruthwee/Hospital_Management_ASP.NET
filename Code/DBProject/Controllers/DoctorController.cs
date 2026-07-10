using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace DBProject.Controllers
{
    public class DoctorController : Controller
    {
        public IActionResult Index()
        {
            // Fixed cr-dotnet-0045: Using distributed session
            var doctorId = HttpContext.Session.GetString("DoctorId");
            return View();
        }

        public IActionResult Bill()
        {
            // Fixed cr-dotnet-0045: Using distributed session
            var doctorId = HttpContext.Session.GetString("DoctorId");
            return View();
        }

        public IActionResult PatientHistory()
        {
            // Fixed cr-dotnet-0045: Using distributed session
            var doctorId = HttpContext.Session.GetString("DoctorId");
            return View();
        }

        public IActionResult HistoryUpdate()
        {
            // Fixed cr-dotnet-0045: Using distributed session
            var doctorId = HttpContext.Session.GetString("DoctorId");
            return View();
        }

        public IActionResult PendingAppointment()
        {
            // Fixed cr-dotnet-0045: Using distributed session
            var doctorId = HttpContext.Session.GetString("DoctorId");
            return View();
        }

        public IActionResult PreviousHistory()
        {
            // Fixed cr-dotnet-0045: Using distributed session
            var doctorId = HttpContext.Session.GetString("DoctorId");
            return View();
        }
    }
}