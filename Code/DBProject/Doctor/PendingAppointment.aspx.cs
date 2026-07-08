using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Pages.Doctor
{
    public class PendingAppointmentModel : PageModel
    {
        private readonly HospitalManagement.Models.ApplicationDbContext _context;

        public PendingAppointmentModel(HospitalManagement.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Appointment> Appointments { get; set; } = new();

        public async Task OnGetAsync()
        {
            Appointments = await _context.Appointments
                .Where(a => a.Status == "Pending")
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.Status = "Approved";
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }

    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
