using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoardGameCompany.Data;
using BoardGameCompany.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BoardGameCompany.Pages.StoreFront.Review
{
    [Authorize(Roles = "Admin,Customer")]
    public class EditModel : PageModel
    {
        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;

        public EditModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [BindProperty]
        public UserComment UserComment { get; set; }
        [BindProperty]
        public BoardGame BoardGame { get; set; }

        public async Task<IActionResult> OnGetAsync(
            string userId, 
            int boardGameId,
            int commentId)
        {
            if(User.Identity.IsAuthenticated)
            {
                if (userId == null)
                {
                    return NotFound();
                }

                var user = await _userManager.FindByEmailAsync(User.Identity.Name);

                UserComment = await _db.UserComment
                    .Where(u => u.UserIdBoardGameComment == user.Id)
                    .FirstOrDefaultAsync(u => u.ID == commentId);

                BoardGame = await _db.BoardGame
                    .FirstOrDefaultAsync(b => b.ID == boardGameId);

                if (UserComment == null || BoardGame == null)
                {
                    return NotFound();
                }
                
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int commentId)
        {
            var commentToUpdate = await _db.UserComment.FindAsync(commentId);

            if (commentToUpdate == null)
            {
                return NotFound();
            }

            var boardGameId = commentToUpdate.BoardGameID;

            await TryUpdateModelAsync<UserComment>(
                commentToUpdate,
                "usercomment",
                c => c.UserBoardGameCommentDate,
                c => c.UserIdBoardGameComment,
                c => c.BoardGameID,
                c => c.UserBoardGameComment);
            {
                await _db.SaveChangesAsync();
                return RedirectToPage("../Index", new { id = boardGameId });
            }

            
        }
    }
}
