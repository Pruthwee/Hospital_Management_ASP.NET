using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Pages.Admin
{
    public class ManageClinicModel : PageModel
    {
        private readonly HospitalManagement.Models.ApplicationDbContext _context;

        public ManageClinicModel(HospitalManagement.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Clinic> Clinics { get; set; } = new();

        public async Task OnGetAsync()
        {
            Clinics = await _context.Clinics.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var clinic = await _context.Clinics.FindAsync(id);
            if (clinic != null)
            {
                _context.Clinics.Remove(clinic);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }

    public class Clinic
    {
        public int ClinicId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }
}
