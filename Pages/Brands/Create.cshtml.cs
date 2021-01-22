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
    public class CreateModel : PageModel
    {
        private ApplicationDbContext _db;

        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Brand Brand { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var emptyBrand = new Brand();

            if (await TryUpdateModelAsync<Brand>(
                emptyBrand,
                "brand",
                b => b.BrandName))
            {
                _db.Brands.Add(emptyBrand);
                await _db.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
