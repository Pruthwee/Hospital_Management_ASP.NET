using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace HospitalManagement.Pages
{
    public class SignUpModel : PageModel
    {
        private readonly HospitalManagement.Models.ApplicationDbContext _context;
        private readonly IDistributedCache _cache;

        public SignUpModel(HospitalManagement.Models.ApplicationDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [BindProperty]
        public User User { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            _context.Users.Add(User);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }

    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
