using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoardGameCompany.Data;
using BoardGameCompany.Models;
using BoardGameCompany.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BoardGameCompany.Pages.Users
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class EditModel : PageModel
    {
        private ApplicationDbContext _db;

        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public ApplicationUser ApplicationUser { get; set; }

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id.Trim().Length == 0)
            {
                return NotFound();
            }

            ApplicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);

            if (ApplicationUser == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            // var applicationUserUpdate = await _db.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);
            var applicationUserUpdate = await _db.ApplicationUser.FindAsync(id);

            if (applicationUserUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<ApplicationUser>(
                applicationUserUpdate,
                "applicationuser",
                a => a.Name,
                a => a.PhoneNumber,
                a => a.Email))
            {
                await _db.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            return Page();
        } 
    }
}
