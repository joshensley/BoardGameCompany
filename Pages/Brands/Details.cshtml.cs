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
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public DetailsModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public Brand Brand { get; set; }

        // public IList<BoardGame> BoardGames { get; set; }
        public IList<InventoryItem> InventoryItems { get; set; }


        public async Task OnGet(int? id)
        {
            // Query
            Brand = await _db.Brands
                .Include(b => b.BoardGames)
                    .ThenInclude(b => b.InventoryItems.Where(i => i.InStock == true))
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

        }
        

        
    }
}
