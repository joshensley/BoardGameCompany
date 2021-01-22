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

namespace BoardGameCompany.Pages.BoardGames
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        // Sort
        public string UPCSort { get; set; }
        public string TitleSort { get; set; }
        public string PlayerNumberSort { get; set; }
        public string ReleaseDateSort { get; set; }
        public string PriceSort { get; set; }
        public string CurrentSort { get; set; }
        public string InStock { get; set; }

        // Filter
        public string CurrentFilter { get; set; }

        // Properties
        public PaginatedList<BoardGame> BoardGames { get; set; }

        public async Task OnGet(
            string sortOrder,
            string currentFilter, 
            string searchString, 
            int? pageIndex)
        {
            // Sort
            CurrentSort = sortOrder;
            UPCSort = sortOrder == "upc_desc" ? "upc_asc" : "upc_desc";
            TitleSort = sortOrder == "title_desc" ? "title_asc" : "title_desc";
            PlayerNumberSort = sortOrder == "player_desc" ? "player_asc" : "player_desc";
            ReleaseDateSort = sortOrder == "release_date_desc" ? "release_date_asc" : "release_date_desc";
            PriceSort = sortOrder == "price_desc" ? "price_asc" : "price_desc";
            InStock = sortOrder == "in_stock_desc" ? "in_stock_asc" : "in_stock_desc";

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

            IQueryable<BoardGame> boardGamesIQ = _db.BoardGame
                .Include(b => b.InventoryItems.Where(i => i.InStock == true));

            if (!String.IsNullOrEmpty(searchString))
            {
                boardGamesIQ = boardGamesIQ.Where(b => b.Title.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "upc_desc":
                    boardGamesIQ = boardGamesIQ.OrderByDescending(b => b.UPC);
                    break;
                case "upc_asc":
                    boardGamesIQ = boardGamesIQ.OrderBy(b => b.UPC);
                    break;
                case "title_desc":
                    boardGamesIQ = boardGamesIQ.OrderByDescending(b => b.Title);
                    break;
                case "title_asc":
                    boardGamesIQ = boardGamesIQ.OrderBy(b => b.Title);
                    break;
                case "player_desc":
                    boardGamesIQ = boardGamesIQ.OrderByDescending(b => b.PlayerNumber);
                    break;
                case "player_asc":
                    boardGamesIQ = boardGamesIQ.OrderBy(b => b.PlayerNumber);
                    break;
                case "release_date_desc":
                    boardGamesIQ = boardGamesIQ.OrderByDescending(b => b.ReleaseDate);
                    break;
                case "release_date_asc":
                    boardGamesIQ = boardGamesIQ.OrderBy(b => b.ReleaseDate);
                    break;
                case "price_desc":
                    boardGamesIQ = boardGamesIQ.OrderByDescending(b => b.Price);
                    break;
                case "price_asc":
                    boardGamesIQ = boardGamesIQ.OrderBy(b => b.Price);
                    break;
                case "in_stock_desc":
                    boardGamesIQ = boardGamesIQ.OrderByDescending(b => b.InventoryItems.Where(i => i.InStock == true).Count());
                    break;
                case "in_stock_asc":
                    boardGamesIQ = boardGamesIQ.OrderBy(b => b.InventoryItems.Where(i => i.InStock == true).Count());
                    break;
                default:
                    boardGamesIQ = boardGamesIQ.OrderBy(b => b.Title);
                    break;
            }

            int pageSize = 5;
            BoardGames = await PaginatedList<BoardGame>.CreateAsync(
                boardGamesIQ.AsNoTracking(), pageIndex ?? 1, pageSize);

        }
    }
}
