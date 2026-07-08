using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Pages.Patient
{
    public class DoctorProfileModel : PageModel
    {
        private readonly HospitalManagement.Models.ApplicationDbContext _context;

        public DoctorProfileModel(HospitalManagement.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public DoctorRecord Doctor { get; set; }

        public async Task OnGetAsync(int doctorId)
        {
            Doctor = await _context.Doctors.FindAsync(doctorId);
        }
    }

    public class DoctorRecord
    {
        public int DoctorId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
