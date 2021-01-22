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
    public class CreateShippingInformationModel : PageModel
    {
        public ApplicationDbContext _db;
        public readonly UserManager<ApplicationUser> _userManager;

        public CreateShippingInformationModel(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // Shipping Information
        public UserShippingInformation UserShippingInformation { get; set; }

        // Shipping Information
        public async Task<IActionResult> OnPostAsync()
        {
            var emptyUserShippingInformation = new UserShippingInformation();

            var user = await _userManager.FindByEmailAsync(User.Identity.Name);

            await TryUpdateModelAsync<UserShippingInformation>(
                emptyUserShippingInformation,
                "usershippinginformation",
                 u => u.Name,
                 u => u.Address1,
                 u => u.Address2,
                 u => u.City,
                 u => u.State,
                 u => u.PostalCode,
                 u => u.Country,
                 u => u.ApplicationUserId);
            {
                emptyUserShippingInformation.ApplicationUserId = user.Id;
                _db.UserShippingInformation.Add(emptyUserShippingInformation);
                await _db.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
        }
    }
}
