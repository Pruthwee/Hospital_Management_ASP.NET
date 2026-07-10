using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace DBProject.Controllers
{
    public class PatientController : Controller
    {
        public IActionResult Index()
        {
            // Fixed cr-dotnet-0045: Using distributed session
            var userId = HttpContext.Session.GetString("UserId");
            return View();
        }

        public IActionResult AppointmentRequestSent()
        {
            // Fixed cr-dotnet-0045: Using distributed session
            var userId = HttpContext.Session.GetString("UserId");
            return View();
        }

        public IActionResult TakeAppointment()
        {
            // Fixed cr-dotnet-0045: Using distributed session
            var userId = HttpContext.Session.GetString("UserId");
            return View();
        }

        public IActionResult BillsHistory()
        {
            // Fixed cr-dotnet-0045: Using distributed session
            var userId = HttpContext.Session.GetString("UserId");
            return View();
        }

        public IActionResult CurrentAppointment()
        {
            // Fixed cr-dotnet-0045: Using distributed session
            var userId = HttpContext.Session.GetString("UserId");
            return View();
        }

        public IActionResult DoctorProfile()
        {
            // Fixed cr-dotnet-0045: Using distributed session
            var userId = HttpContext.Session.GetString("UserId");
            return View();
        }

        public IActionResult Feedback()
        {
            // Fixed cr-dotnet-0045: Using distributed session
            var userId = HttpContext.Session.GetString("UserId");
            return View();
        }

        public IActionResult Notifications()
        {
            // Fixed cr-dotnet-0045: Using distributed session
            var userId = HttpContext.Session.GetString("UserId");
            return View();
        }

        public IActionResult TreatmentHistory()
        {
            // Fixed cr-dotnet-0045: Using distributed session
            var userId = HttpContext.Session.GetString("UserId");
            return View();
        }

        public IActionResult ViewDoctors()
        {
            // Fixed cr-dotnet-0045: Using distributed session
            var userId = HttpContext.Session.GetString("UserId");
            return View();
        }
    }
}