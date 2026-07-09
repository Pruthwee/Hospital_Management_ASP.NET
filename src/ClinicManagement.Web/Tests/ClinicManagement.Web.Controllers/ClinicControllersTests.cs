using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ClinicManagement.Web.Controllers;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Linq;

namespace ClinicManagement.Web.Tests.Controllers
{
    public class PatientControllerTests
    {
        private readonly Mock<IPatientService> _mockPatientService;
        private readonly PatientController _controller;

        public PatientControllerTests()
        {
            _mockPatientService = new Mock<IPatientService>();
            _controller = new PatientController(_mockPatientService.Object);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public async Task Index_ReturnsViewResult()
        {
            var result = await _controller.Index();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Profile_ReturnsViewWithPatient()
        {
            int patientId = 1;
            var patient = new Patient { PatientID = patientId, Name = "Test Patient" };
            _mockPatientService.Setup(s => s.GetPatientByIdAsync(patientId)).ReturnsAsync(patient);

            var result = await _controller.Profile(patientId);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(patient, viewResult.Model);
        }

        [Fact]
        public async Task ViewDoctors_ReturnsViewWithDoctors()
        {
            string dept = "Cardiology";
            var doctors = new List<Doctor> { new Doctor { Name = "Dr. Smith" } };
            _mockPatientService.Setup(s => s.GetDoctorsByDepartmentAsync(dept)).ReturnsAsync(doctors);

            var result = await _controller.ViewDoctors(dept);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(doctors, viewResult.Model);
        }

        [Fact]
        public async Task TakeAppointment_ReturnsViewWithSlots()
        {
            int doctorId = 10;
            var slots = new List<int> { 1, 2, 3 };
            _mockPatientService.Setup(s => s.GetFreeSlotsAsync(doctorId, 1)).ReturnsAsync(slots);

            var result = await _controller.TakeAppointment(doctorId);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(slots, viewResult.Model);
        }

        [Fact]
        public async Task RequestAppointment_RedirectsToIndex()
        {
            int doctorId = 10;
            int slot = 1;
            _mockPatientService.Setup(s => s.RequestAppointmentAsync(doctorId, 1, slot)).ReturnsAsync(1);

            var result = await _controller.RequestAppointment(doctorId, slot);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task History_ReturnsViewWithHistory()
        {
            int patientId = 1;
            var history = new List<Appointment> { new Appointment { AppointmentID = 1 } };
            _mockPatientService.Setup(s => s.GetTreatmentHistoryAsync(patientId)).ReturnsAsync(history);

            var result = await _controller.History(patientId);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(history, viewResult.Model);
        }
    }

    public class DoctorControllerTests
    {
        private readonly Mock<IDoctorService> _mockDoctorService;
        private readonly DoctorController _controller;

        public DoctorControllerTests()
        {
            _mockDoctorService = new Mock<IDoctorService>();
            _controller = new DoctorController(_mockDoctorService.Object);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public async Task Index_ReturnsViewResult()
        {
            var result = await _controller.Index();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task PendingAppointments_ReturnsViewWithAppointments()
        {
            int doctorId = 10;
            var apps = new List<Appointment> { new Appointment { AppointmentID = 1 } };
            _mockDoctorService.Setup(s => s.GetPendingAppointmentsAsync(doctorId)).ReturnsAsync(apps);

            var result = await _controller.PendingAppointments(doctorId);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(apps, viewResult.Model);
        }

        [Fact]
        public async Task Approve_RedirectsToPendingAppointments()
        {
            int appointmentId = 1;
            _mockDoctorService.Setup(s => s.ApproveAppointmentAsync(appointmentId)).ReturnsAsync(true);

            var result = await _controller.Approve(appointmentId);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("PendingAppointments", redirectResult.ActionName);
        }

        [Fact]
        public async Task TodaysAppointments_ReturnsViewWithAppointments()
        {
            int doctorId = 10;
            var apps = new List<Appointment> { new Appointment { AppointmentID = 1 } };
            _mockDoctorService.Setup(s => s.GetTodaysAppointmentsAsync(doctorId)).ReturnsAsync(apps);

            var result = await _controller.TodaysAppointments(doctorId);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(apps, viewResult.Model);
        }

        [Fact]
        public async Task UpdatePrescription_RedirectsToTodaysAppointments()
        {
            int appointmentId = 1;
            string disease = "Flu";
            string progress = "Improving";
            string prescription = "Rest";
            _mockDoctorService.Setup(s => s.UpdatePrescriptionAsync(1, appointmentId, disease, progress, prescription)).ReturnsAsync(true);

            var result = await _controller.UpdatePrescription(appointmentId, disease, progress, prescription);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("TodaysAppointments", redirectResult.ActionName);
        }
    }

    public class AdminControllerTests
    {
        private readonly Mock<IAdminService> _mockAdminService;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _mockAdminService = new Mock<IAdminService>();
            _controller = new AdminController(_mockAdminService.Object);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public async Task Index_ReturnsViewResult()
        {
            var result = await _controller.Index();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ManageDoctors_ReturnsViewWithDoctors()
        {
            string search = "Smith";
            var doctors = new List<Doctor> { new Doctor { Name = "Dr. Smith" } };
            _mockAdminService.Setup(s => s.GetDoctorsAsync(search)).ReturnsAsync(doctors);

            var result = await _controller.ManageDoctors(search);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(doctors, viewResult.Model);
        }

        [Fact]
        public async Task AddDoctor_RedirectsToManageDoctors()
        {
            var doctor = new Doctor { Name = "New Doctor" };
            _mockAdminService.Setup(s => s.AddDoctorAsync(doctor)).ReturnsAsync(1);

            var result = await _controller.AddDoctor(doctor);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ManageDoctors", redirectResult.ActionName);
        }

        [Fact]
        public async Task ManageStaff_ReturnsViewWithStaff()
        {
            string search = "Nurse";
            var staff = new List<Staff> { new Staff { Name = "Nurse Joy" } };
            _mockAdminService.Setup(s => s.GetStaffAsync(search)).ReturnsAsync(staff);

            var result = await _controller.ManageStaff(search);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(staff, viewResult.Model);
        }

        [Fact]
        public async Task AddStaff_RedirectsToManageStaff()
        {
            var staff = new Staff { Name = "New Staff" };
            _mockAdminService.Setup(s => s.AddStaffAsync(staff)).ReturnsAsync(1);

            var result = await _controller.AddStaff(staff);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ManageStaff", redirectResult.ActionName);
        }
    }
}
