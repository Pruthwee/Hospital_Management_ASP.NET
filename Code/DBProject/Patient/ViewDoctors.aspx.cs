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
    public class ViewDoctorsModel : PageModel
    {
        private readonly HospitalManagement.Models.ApplicationDbContext _context;
        private readonly IDistributedCache _cache;

        public ViewDoctorsModel(HospitalManagement.Models.ApplicationDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public List<DoctorRecord> Doctors { get; set; } = new();

        public async Task OnGetAsync()
        {
            Doctors = await _context.Doctors.ToListAsync();
        }
    }
}
