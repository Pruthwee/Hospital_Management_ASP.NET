using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace ClinicManagement.Web.Controllers
{
    [Authorize(Roles = "Patient")]
    public class PatientController : Controller
    {
        private readonly IPatientService _patientService;
        public PatientController(IPatientService patientService) { _patientService = patientService; }

        public async Task<IActionResult> Index() => View();

        public async Task<IActionResult> Profile(int id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);
            return View(patient);
        }

        public async Task<IActionResult> ViewDoctors(string dept)
        {
            var doctors = await _patientService.GetDoctorsByDepartmentAsync(dept);
            return View(doctors);
        }

        public async Task<IActionResult> TakeAppointment(int doctorId)
        {
            var slots = await _patientService.GetFreeSlotsAsync(doctorId, 1); // Mock patient ID 1
            return View(slots);
        }

        [HttpPost]
        public async Task<IActionResult> RequestAppointment(int doctorId, int slot)
        {
            await _patientService.RequestAppointmentAsync(doctorId, 1, slot);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> History(int id)
        {
            var history = await _patientService.GetTreatmentHistoryAsync(id);
            return View(history);
        }
    }

    [Authorize(Roles = "Doctor")]
    public class DoctorController : Controller
    {
        private readonly IDoctorService _doctorService;
        public DoctorController(IDoctorService doctorService) { _doctorService = doctorService; }

        public async Task<IActionResult> Index() => View();

        public async Task<IActionResult> PendingAppointments(int doctorId)
        {
            var apps = await _doctorService.GetPendingAppointmentsAsync(doctorId);
            return View(apps);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int appointmentId)
        {
            await _doctorService.ApproveAppointmentAsync(appointmentId);
            return RedirectToAction("PendingAppointments");
        }

        public async Task<IActionResult> TodaysAppointments(int doctorId)
        {
            var apps = await _doctorService.GetTodaysAppointmentsAsync(doctorId);
            return View(apps);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePrescription(int appointmentId, string disease, string progress, string prescription)
        {
            await _doctorService.UpdatePrescriptionAsync(1, appointmentId, disease, progress, prescription); // Mock doctor ID 1
            return RedirectToAction("TodaysAppointments");
        }
    }

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService) { _adminService = adminService; }

        public async Task<IActionResult> Index() => View();

        public async Task<IActionResult> ManageDoctors(string search)
        {
            var doctors = await _adminService.GetDoctorsAsync(search);
            return View(doctors);
        }

        [HttpPost]
        public async Task<IActionResult> AddDoctor(Doctor doctor)
        {
            await _adminService.AddDoctorAsync(doctor);
            return RedirectToAction("ManageDoctors");
        }

        public async Task<IActionResult> ManageStaff(string search)
        {
            var staff = await _adminService.GetStaffAsync(search);
            return View(staff);
        }

        [HttpPost]
        public async Task<IActionResult> AddStaff(Staff staff)
        {
            await _adminService.AddStaffAsync(staff);
            return RedirectToAction("ManageStaff");
        }
    }
}
