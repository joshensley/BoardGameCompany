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
    public class EditModel : PageModel
    {
        private ApplicationDbContext _db;

        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public InventoryItem InventoryItem { get; set; }
        public IList<BoardGame> BoardGames { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            InventoryItem = await _db.InventoryItem.FindAsync(id);
            BoardGames = await _db.BoardGame.OrderBy(b => b.Title).ToListAsync();

            if (InventoryItem == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var inventoryItemUpdate = await _db.InventoryItem.FindAsync(id);

            if(inventoryItemUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<InventoryItem>(
                inventoryItemUpdate,
                "inventoryitem",
                i => i.ID,
                i => i.InStock,
                i => i.ReceivedDate,
                i => i.ShippedDate,
                i => i.Condition,
                i => i.BoardGameID))
            {
                await _db.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            return Page();
        }
    }
}
