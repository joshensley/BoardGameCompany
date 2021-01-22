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
using Microsoft.AspNetCore.Identity;

namespace BoardGameCompany.Pages.StoreFront.Review
{
    [Authorize(Roles = "Admin,Customer")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public DeleteModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [BindProperty]
        public UserComment UserComment { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? commentId, bool? saveChangesError = false)
        {

            if (commentId == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByEmailAsync(User.Identity.Name);

            UserComment = await _db.UserComment
                .Where(u => u.UserIdBoardGameComment == user.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ID == commentId);            

            if (UserComment == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ErrorMessage = "Error";
            }

            return Page();


        }

        public async Task<IActionResult> OnPostAsync(
            int? boardGameId,
            int? commentId,
            string? userId)
        {
            if (commentId == null)
            {
                return NotFound();
            }

            var userComment = await _db.UserComment
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ID == commentId);

            if (userComment == null)
            {
                return NotFound();
            }

            try
            {
                _db.UserComment.Remove(userComment);
                await _db.SaveChangesAsync();
                return RedirectToPage("../Index", new { id = boardGameId });
            }
            catch(DbUpdateException)
            {
                return RedirectToAction("./Delete");
            }
        }
    }
}
