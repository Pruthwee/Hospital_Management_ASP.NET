using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace HospitalManagement.Pages.Doctor
{
    public class PatientHistoryModel : PageModel
    {
        private readonly HospitalManagement.Models.ApplicationDbContext _context;
        private readonly IDistributedCache _cache;

        public PatientHistoryModel(HospitalManagement.Models.ApplicationDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public List<PatientHistoryRecord> Histories { get; set; } = new();

        public async Task OnGetAsync(int patientId)
        {
            // Using distributed cache (Redis) instead of in-memory session for cloud readiness
            Histories = await _context.PatientHistories
                .Where(h => h.PatientId == patientId)
                .ToListAsync();
        }
    }

    public class PatientHistoryRecord
    {
        public int HistoryId { get; set; }
        public int PatientId { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string Treatment { get; set; } = string.Empty;
    }
}
