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
    public class CreateModel : PageModel
    {
        private IWebHostEnvironment _environment;
        private ApplicationDbContext _db;

        public CreateModel(ApplicationDbContext db, IWebHostEnvironment environment)
        {
            _environment = environment;
            _db = db;
        }

        [BindProperty]
        public BoardGame BoardGame { get; set; }
        [BindProperty]
        public IList<Brand> Brands { get; set; }
        [BindProperty]
        public IFormFile Upload { get; set; }
        public string FilePath { get; set; }

        public async Task OnGetAsync()
        {
            Brands = await _db.Brands.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var file = Path.Combine(_environment.ContentRootPath, "wwwroot/images/boardgames/", Upload.FileName);
            FilePath = $"/images/boardgames/{Upload.FileName}";

            using (var fileStream = new FileStream(file, FileMode.Create))
            {
                await Upload.CopyToAsync(fileStream);
            }

            var emptyBoardGame = new BoardGame();
            

            if(await TryUpdateModelAsync<BoardGame>(
                emptyBoardGame,
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
                emptyBoardGame.BoardGameImageFilePath = FilePath;
                _db.BoardGame.Add(emptyBoardGame);
                await _db.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
