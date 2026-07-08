using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Pages.Doctor
{
    public class HistoryUpdateModel : PageModel
    {
        private readonly HospitalManagement.Models.ApplicationDbContext _context;

        public HistoryUpdateModel(HospitalManagement.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public PatientHistory History { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            _context.PatientHistories.Update(History);
            await _context.SaveChangesAsync();

            return RedirectToPage("DoctorHome");
        }
    }

    public class PatientHistory
    {
        public int HistoryId { get; set; }
        public int PatientId { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string Treatment { get; set; } = string.Empty;
    }
}
