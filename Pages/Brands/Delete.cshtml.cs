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

namespace BoardGameCompany.Pages.Brands
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public DeleteModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Brand Brand { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, bool? savechangesError = false)
        {
            if(id == null)
            {
                return NotFound();
            }

            Brand = await _db.Brands
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.ID == id);

            if(Brand == null)
            {
                return NotFound();
            }

            if (savechangesError.GetValueOrDefault())
            {
                ErrorMessage = "Delete is restricted. Delete all associated children and then delete Brand.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var brand = await _db.Brands.FindAsync(id);

            if (brand == null)
            {
                return NotFound();
            }

            try
            {
                _db.Brands.Remove(brand);
                await _db.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException)
            {
                return RedirectToAction("./Delete", new { id, saveChangesError = true });
            }
        }
    }
}
