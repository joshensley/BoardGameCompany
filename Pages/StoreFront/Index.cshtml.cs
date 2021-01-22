using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BoardGameCompany.Data;
using BoardGameCompany.Models;
using BoardGameCompany.Pages.StoreFront.UserCommentsViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using static BoardGameCompany.Pagination;

namespace BoardGameCompany.Pages.StoreFront
{
    public class IndexModel : PageModel
    {
        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;

        public IndexModel(
            ApplicationDbContext db, 
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [BindProperty]
        public BoardGame BoardGame { get; set; }
        [BindProperty]
        public UserComment UserComment { get; set; }
        [BindProperty]
        public IList<UserCart> UserCartList { get; set; }
        [BindProperty]
        public UserCart UserCart { get; set; }
        public PaginatedList<UserCommentGroup> UserComments { get; set; }
        public string UserId { get; set; }
        public bool UserHasReview { get; set; }
        public int UserCommentId { get; set; }
        public string ErrorMessage { get; set; }

        public async Task OnGetAsync(
            int id, 
            int? pageIndex,
            int? currentStock,
            bool? quantityError = false)
        {

            // Get User Id
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                UserId = user.Id;

                // Display user cart information
                IQueryable<UserCart> userCartIQ = _db.UserCart
                    .Where(u => u.ApplicationUserId == UserId)
                    .Where(u => u.BoardGameID == id);

                UserCartList = await userCartIQ.AsNoTracking().ToListAsync();
            }


            // Display board game information
            BoardGame = await _db.BoardGame
                .Include(b => b.Brand)
                .Include(b => b.InventoryItems.Where(i => i.InStock == true))
                .FirstOrDefaultAsync(m => m.ID == id);

            if (User.Identity.IsAuthenticated)
            {
                // Used when current user submits a comment
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                UserId = user.Id;

                // Finds if the user wrote a review
                var userHasReview = await _db.UserComment
                    .FirstOrDefaultAsync(
                        u => u.UserIdBoardGameComment == UserId &&
                        u.BoardGameID == id);

                
                UserHasReview = userHasReview != null ? true : false;

                if (UserHasReview)
                {
                    UserCommentId = userHasReview.ID;
                }
                
            }

            // Joins UserComments table with ApplicationUser information
            // Used for displaying user reviews
            IQueryable<UserCommentGroup> res = _db.UserComment
                .Where(u => u.BoardGameID == id)
                .Join(_db.ApplicationUser,
                    userComment => userComment.UserIdBoardGameComment,
                    applicationUser => applicationUser.Id,
                    (userComment, applicationUser) => new UserCommentGroup() 
                    {
                        Id = applicationUser.Id,
                        Name = applicationUser.Name,
                        BoardGameID = userComment.BoardGameID,
                        UserComment = userComment.UserBoardGameComment,
                        CommentDate = userComment.UserBoardGameCommentDate,
                        UserAvatarFilePath = applicationUser.UserAvatarFilePath
                    });

            int pageSize = 5;
            UserComments = await PaginatedList<UserCommentGroup>.CreateAsync(
                res.OrderByDescending(r => r.CommentDate), pageIndex ?? 1, pageSize);

            if(quantityError.GetValueOrDefault())
            {
                ErrorMessage = $"Only {currentStock} in stock";
            }

        }

        public async Task<IActionResult> OnPostCartAsync(
            int? boardGameId,
            decimal price)
        {

            if(boardGameId == null)
            {
                return NotFound();
            }

            int currentStock = _db.InventoryItem
                .Where(i => i.BoardGameID == boardGameId)
                .Where(i => i.InStock == true)
                .Count();

            var emptyUserCart = new UserCart();

            await TryUpdateModelAsync<UserCart>(
                emptyUserCart,
                "usercart",
                u => u.Quantity,
                u => u.TotalPrice,
                u => u.BoardGameID,
                u => u.ApplicationUserId);
            {

                if (emptyUserCart.Quantity > currentStock)
                {
                    return RedirectToPage("./Index", new
                    {
                        quantityError = true,
                        currentStock,
                        id = boardGameId
                    });
                }

                emptyUserCart.TotalPrice = price * emptyUserCart.Quantity;

                _db.UserCart.Add(emptyUserCart);
                await _db.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var emptyUserComment = new UserComment();

            await TryUpdateModelAsync<UserComment>(
                emptyUserComment,
                "usercomment",
                u => u.UserBoardGameComment,
                u => u.UserBoardGameCommentDate,
                u => u.BoardGameID,
                u => u.UserIdBoardGameComment);
            {
                _db.UserComment.Add(emptyUserComment);
                await _db.SaveChangesAsync();
                return RedirectToPage("./Index", new { id = id });
            }
        }


       
    }
}
