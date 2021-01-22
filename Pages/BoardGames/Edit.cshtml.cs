using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BoardGameCompany.Data;
using BoardGameCompany.Models;
using BoardGameCompany.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BoardGameCompany.Pages.BoardGames
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class EditModel : PageModel
    {
        private readonly IWebHostEnvironment _environment;
        private ApplicationDbContext _db;

        public EditModel(ApplicationDbContext db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }

        [BindProperty]
        public BoardGame BoardGame { get; set; }
        public IList<Brand> Brands { get; set; }
        public string FilePath { get; set; }
        public IFormFile Upload { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            BoardGame = await _db.BoardGame
                .Include(b => b.Brand)
                .FirstOrDefaultAsync(m => m.ID == id);

            Brands = await _db.Brands.ToListAsync();

            if(BoardGame == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var boardGameToUpdate = await _db.BoardGame.FindAsync(id);

            if(boardGameToUpdate == null)
            {
                return NotFound();
            }

            if(Upload != null)
            {
                var file = Path.Combine(_environment.ContentRootPath, "wwwroot/images/boardgames/", Upload.FileName);
                FilePath = $"/images/boardgames/{Upload.FileName}";

                using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    await Upload.CopyToAsync(fileStream);
                }

                var deleteFile = Path.Combine(_environment.ContentRootPath, $"wwwroot{boardGameToUpdate.BoardGameImageFilePath}");
                Debug.WriteLine(deleteFile);
                if (System.IO.File.Exists(deleteFile))
                {
                    System.IO.File.Delete(deleteFile);
                }
            }

            if(await TryUpdateModelAsync<BoardGame>(
                boardGameToUpdate,
                "boardgame",
                b => b.UPC,
                b => b.Title,
                b => b.Description,
                b => b.PlayerNumber,
                b => b.ReleaseDate,
                b => b.Price,
                b => b.BrandID,
                b => b.BoardGameImageFilePath))
            {

                if (Upload != null)
                {
                    boardGameToUpdate.BoardGameImageFilePath = FilePath;
                }

                await _db.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            return Page();
        }
    }
}
