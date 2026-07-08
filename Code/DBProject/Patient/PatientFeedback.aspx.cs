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
    public class PatientFeedbackModel : PageModel
    {
        private readonly HospitalManagement.Models.ApplicationDbContext _context;
        private readonly IDistributedCache _cache;

        public PatientFeedbackModel(HospitalManagement.Models.ApplicationDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [BindProperty]
        public Feedback Feedback { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            _context.Feedbacks.Add(Feedback);
            await _context.SaveChangesAsync();

            return RedirectToPage("PatientHome");
        }
    }

    public class Feedback
    {
        public int FeedbackId { get; set; }
        public int PatientId { get; set; }
        public string Comments { get; set; } = string.Empty;
        public int Rating { get; set; }
    }
}
