using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Models
{
    public class Staff
    {
        public int StaffId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Staff> Staffs { get; set; }
    }
}

namespace HospitalManagement.Pages.Admin
{
    public class AddStaffModel : PageModel
    {
        private readonly HospitalManagement.Models.ApplicationDbContext _context;

        public AddStaffModel(HospitalManagement.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public HospitalManagement.Models.Staff Staff { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            _context.Staffs.Add(Staff);
            await _context.SaveChangesAsync();

            return RedirectToPage("AdminHome");
        }
    }
}
