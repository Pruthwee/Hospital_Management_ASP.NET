using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DBProject.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult AddStaff() => View();

        [HttpPost]
        public IActionResult AddStaff(StaffModel model)
        {
            if (ModelState.IsValid)
            {
                // Logic to add staff
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult ManageClinic() => View();

        public IActionResult DoctorRegistration() => View();

        [HttpPost]
        public IActionResult DoctorRegistration(DoctorModel model)
        {
            if (ModelState.IsValid)
            {
                // Logic to register doctor
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }

    public class StaffModel { public string Name { get; set; } = ""; public string Role { get; set; } = ""; }
    public class DoctorModel { public string Name { get; set; } = ""; public string Specialization { get; set; } = ""; }
}