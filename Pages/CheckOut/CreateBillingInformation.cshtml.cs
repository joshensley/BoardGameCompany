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

namespace BoardGameCompany.Pages.CheckOut
{
    [Authorize(Roles = "Admin,Customer")]
    public class CreateBillingInformationModel : PageModel
    {

        public ApplicationDbContext _db;
        public readonly UserManager<ApplicationUser> _userManager;

        public CreateBillingInformationModel(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // Shipping Information
        public UserBillingInformation UserBillingInformation { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var emptyUserBillingInformation = new UserBillingInformation();

            var user = await _userManager.FindByEmailAsync(User.Identity.Name);

            await TryUpdateModelAsync<UserBillingInformation>(
                emptyUserBillingInformation,
                "userbillinginformation",
                u => u.Name,
                u => u.Address1,
                u => u.Address2,
                u => u.City,
                u => u.State,
                u => u.PostalCode,
                u => u.Country,
                u => u.ApplicationUserId);
            {
                emptyUserBillingInformation.ApplicationUserId = user.Id;
                _db.UserBillingInformation.Add(emptyUserBillingInformation);
                await _db.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
        }
    }
}
