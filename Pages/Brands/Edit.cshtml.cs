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

namespace BoardGameCompany.Pages.Brands
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class EditModel : PageModel
    {
        private ApplicationDbContext _db;
        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Brand Brand { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            Brand = await _db.Brands.FindAsync(id);

            if(Brand == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var brandToUpdate = await _db.Brands.FindAsync(id);

            if (brandToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Brand>(
                brandToUpdate,
                "brand",
                b => b.BrandName))
            {
                await _db.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();

        }
    }
}
