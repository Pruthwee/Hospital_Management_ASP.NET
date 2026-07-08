using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Pages.Admin
{
    public class DoctorRegistrationFormModel : PageModel
    {
        private readonly HospitalManagement.Models.ApplicationDbContext _context;

        public DoctorRegistrationFormModel(HospitalManagement.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Doctor Doctor { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            _context.Doctors.Add(Doctor);
            await _context.SaveChangesAsync();

            return RedirectToPage("AdminHome");
        }
    }

    public class Doctor
    {
        public int DoctorId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
