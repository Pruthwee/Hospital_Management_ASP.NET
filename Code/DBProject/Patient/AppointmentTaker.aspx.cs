using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace HospitalManagement.Pages.Patient
{
    public class AppointmentTakerModel : PageModel
    {
        private readonly HospitalManagement.Models.ApplicationDbContext _context;
        private readonly IDistributedCache _cache;

        public AppointmentTakerModel(HospitalManagement.Models.ApplicationDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [BindProperty]
        public AppointmentRequest Request { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            _context.AppointmentRequests.Add(Request);
            await _context.SaveChangesAsync();

            return RedirectToPage("AppointmentRequestSent");
        }
    }

    public class AppointmentRequest
    {
        public int RequestId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime Date { get; set; }
    }
}
