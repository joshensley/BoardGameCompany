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

namespace BoardGameCompany.Pages.InventoryItems
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
        public InventoryItem InventoryItem { get; set; }
        public IList<BoardGame> BoardGames { get; set; }

        public async Task OnGetAsync()
        {
            BoardGames = await _db.BoardGame
                .OrderBy(b => b.Title)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var emptyInventoryItem = new InventoryItem();

            if (await TryUpdateModelAsync<InventoryItem>(
                emptyInventoryItem,
                "inventoryitem",
                i => i.InStock,
                i => i.ReceivedDate,
                i => i.ShippedDate,
                i => i.Condition,
                i => i.BoardGameID))
            {
                _db.InventoryItem.Add(emptyInventoryItem);
                await _db.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            return Page();
        }
    }
}
