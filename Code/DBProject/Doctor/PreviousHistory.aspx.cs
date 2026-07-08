using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Pages.Doctor
{
    public class PreviousHistoryModel : PageModel
    {
        private readonly HospitalManagement.Models.ApplicationDbContext _context;

        public PreviousHistoryModel(HospitalManagement.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public List<PatientHistoryRecord> Histories { get; set; } = new();

        public async Task OnGetAsync(int patientId)
        {
            Histories = await _context.PatientHistories
                .Where(h => h.PatientId == patientId)
                .ToListAsync();
        }
    }
}
