using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Pages.Doctor
{
    public class BillModel : PageModel
    {
        private readonly HospitalManagement.Models.ApplicationDbContext _context;

        public BillModel(HospitalManagement.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Bill Bill { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            _context.Bills.Add(Bill);
            await _context.SaveChangesAsync();

            return RedirectToPage("DoctorHome");
        }
    }

    public class Bill
    {
        public int BillId { get; set; }
        public int PatientId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
