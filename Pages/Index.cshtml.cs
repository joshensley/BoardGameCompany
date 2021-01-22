using BoardGameCompany.Data;
using BoardGameCompany.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BoardGameCompany.Pagination;

namespace BoardGameCompany.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(
            ILogger<IndexModel> logger, 
            ApplicationDbContext db, 
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
        }

        public string TitleSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public PaginatedList<BoardGame> BoardGames { get; set; }

        [BindProperty]
        public UserCart UserCart { get; set; }
        [BindProperty]
        public IList<UserCart> UserCartList { get; set; }
        public string UserId { get; set; }

        public IList<int> PageNumberList { get; set; }

        public async Task OnGet(
            int? pageIndex, 
            string currentFilter, 
            string sortOrder, 
            string searchString)
        {
            if (User.Identity.IsAuthenticated)
            {
                // Get User Id
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                UserId = user.Id;

                IQueryable<UserCart> userCartIQ = _db.UserCart
                    .Where(u => u.ApplicationUserId == UserId);

                UserCartList = await userCartIQ.AsNoTracking().ToListAsync();
            }


            // Get Board Games
            CurrentSort = sortOrder;
            TitleSort = sortOrder == "title_desc" ? "title_asc" : "title_desc";


            if (searchString != null)
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
                .Include(b => b.Brand)
                .Include(b => b.InventoryItems.Where(i => i.InStock == true));
                

            if (!String.IsNullOrEmpty(searchString))
            {
                boardGamesIQ = boardGamesIQ.Where(b => b.Title.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "title_asc":
                    boardGamesIQ = boardGamesIQ.OrderBy(b => b.Title);
                    break;
                case "title_desc":
                    boardGamesIQ = boardGamesIQ.OrderByDescending(b => b.Title);
                    break;
                default:
                    boardGamesIQ = boardGamesIQ.OrderBy(b => b.Title);
                    break;
            }

            int pageSize = 6;
            BoardGames = await PaginatedList<BoardGame>.CreateAsync(
                boardGamesIQ.AsNoTracking(), pageIndex ?? 1, pageSize);


            // Pagination
            if((BoardGames.PageIndex == 1) && (BoardGames.TotalPages >= 5))
            {
                PageNumberList = Enumerable.Range(1, 5).ToArray();
            }
            else if ((BoardGames.TotalPages < 5))
            {
                PageNumberList = Enumerable.Range(1, BoardGames.TotalPages).ToArray();
            }


            else if ((BoardGames.PageIndex + 4) >= BoardGames.TotalPages)
            {
                PageNumberList = Enumerable.Range((BoardGames.TotalPages - 5), 6).ToArray();
            }
            else
            {
                PageNumberList = Enumerable.Range(BoardGames.PageIndex, 4).ToArray();
            }

            
        }

        public async Task<IActionResult> OnPostAsync()
        {

            var emptyUserCart = new UserCart();

            if (await TryUpdateModelAsync<UserCart>(
                emptyUserCart,
                "usercart",
                u => u.Quantity,
                u => u.TotalPrice,
                u => u.BoardGameID,
                u => u.ApplicationUserId))
            {
                _db.UserCart.Add(emptyUserCart);
                await _db.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
