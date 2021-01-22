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

namespace BoardGameCompany.Pages.BoardGames
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
        public BoardGame BoardGame { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            BoardGame = await _db.BoardGame
                .AsNoTracking()
                .FirstAsync();

            if (BoardGame == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ErrorMessage = "Delete is restricted. Delete all associated children and then delete Board Game.";
            }

            return Page();

        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boardGame = await _db.BoardGame.FindAsync(id);

            if (boardGame == null)
            {
                return NotFound();
            }

            try
            {
                _db.BoardGame.Remove(boardGame);
                await _db.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            catch
            {
                return RedirectToAction("./Delete", new { id, saveChangesError = true });
            }
        }
    }
}
