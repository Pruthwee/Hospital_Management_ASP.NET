using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Pages.Patient
{
    public class BillsHistoryModel : PageModel
    {
        private readonly HospitalManagement.Models.ApplicationDbContext _context;

        public BillsHistoryModel(HospitalManagement.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public List<BillRecord> Bills { get; set; } = new();

        public async Task OnGetAsync(int patientId)
        {
            Bills = await _context.Bills
                .Where(b => b.PatientId == patientId)
                .ToListAsync();
        }
    }

    public class BillRecord
    {
        public int BillId { get; set; }
        public int PatientId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
