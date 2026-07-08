using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Pages.Patient
{
    public class CurrentAppointmentModel : PageModel
    {
        private readonly HospitalManagement.Models.ApplicationDbContext _context;

        public CurrentAppointmentModel(HospitalManagement.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public AppointmentRecord Appointment { get; set; }

        public async Task OnGetAsync(int patientId)
        {
            Appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.PatientId == patientId && a.Status == "Approved");
        }
    }

    public class AppointmentRecord
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
