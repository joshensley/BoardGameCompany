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
using static BoardGameCompany.Pagination;

namespace BoardGameCompany.Pages.Brands
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public string CurrentFilter { get; set; }
        public PaginatedList<Brand> Brands { get; set; }

        public async Task OnGetAsync(
            int? pageIndex, 
            string searchString,
            string currentFilter)
        {
            if(searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            // Filter
            CurrentFilter = searchString;

            IQueryable<Brand> brandsIQ = _db.Brands.OrderBy(b => b.BrandName);

            if (!String.IsNullOrEmpty(searchString))
            {
                brandsIQ = brandsIQ.Where(b => b.BrandName.Contains(searchString));
            }

            int pageSize = 5;
            Brands = await PaginatedList<Brand>.CreateAsync(
                brandsIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
