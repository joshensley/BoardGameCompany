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

namespace BoardGameCompany.Pages.CheckOut
{
    [Authorize(Roles = "Admin,Customer")]
    public class EditBillingInformationModel : PageModel
    {
        public ApplicationDbContext _db;
        public readonly UserManager<ApplicationUser> _userManager;

        public EditBillingInformationModel(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [BindProperty]
        public UserBillingInformation UserBillingInformation { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);

            UserBillingInformation = await _db.UserBillingInformation
                .FirstOrDefaultAsync(u => u.ApplicationUserId == user.Id);

            if(UserBillingInformation == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var updateUserBillingInformation = await _db.UserBillingInformation.FindAsync(id);

            if (updateUserBillingInformation == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<UserBillingInformation>(
                updateUserBillingInformation,
                "userbillinginformation",
                 u => u.Name,
                 u => u.Address1,
                 u => u.Address2,
                 u => u.City,
                 u => u.State,
                 u => u.PostalCode,
                 u => u.Country,
                 u => u.ApplicationUserId))
            {
                await _db.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
