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
    public class EditShippingInformationModel : PageModel
    {
        public ApplicationDbContext _db;
        public readonly UserManager<ApplicationUser> _userManager;

        public EditShippingInformationModel(
            ApplicationDbContext db, 
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [BindProperty]
        public UserShippingInformation UserShippingInformation { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);

            UserShippingInformation = await _db.UserShippingInformation
                .FirstOrDefaultAsync(u => u.ApplicationUserId == user.Id);

            if(UserShippingInformation == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var updateUserShippingInformation = await _db.UserShippingInformation.FindAsync(id);

            if (updateUserShippingInformation == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<UserShippingInformation>(
                updateUserShippingInformation,
                "usershippinginformation",
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
