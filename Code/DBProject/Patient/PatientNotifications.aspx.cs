using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Pages.Patient
{
    public class PatientNotificationsModel : PageModel
    {
        private readonly HospitalManagement.Models.ApplicationDbContext _context;

        public PatientNotificationsModel(HospitalManagement.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Notification> Notifications { get; set; } = new();

        public async Task OnGetAsync(int patientId)
        {
            Notifications = await _context.Notifications
                .Where(n => n.PatientId == patientId)
                .ToListAsync();
        }
    }

    public class Notification
    {
        public int NotificationId { get; set; }
        public int PatientId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}
