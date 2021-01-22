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
    public class ConfirmationPageModel : PageModel
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public ConfirmationPageModel(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [BindProperty]
        public IList<UserOrderInformation> UserOrderInformation { get; set; }

        [BindProperty]
        public UserOrderInformationPrice UserOrderInformationPrice { get; set; }

        public async Task OnGetAsync(int orderId)
        {

            var user = await _userManager.FindByEmailAsync(User.Identity.Name);

            IQueryable<UserOrderInformation> userOrderInformationIQ = _db.UserOrderInformation
                .Where(u => u.ApplicationUserId == user.Id)
                .Where(u => u.OrderInformationGroupID == orderId)
                .Include(u => u.BoardGame)
                    .ThenInclude(u => u.Brand);

            UserOrderInformation = await userOrderInformationIQ
                .AsNoTracking()
                .ToListAsync();

            UserOrderInformationPrice = await _db.UserOrderInformationPrice
               .FirstOrDefaultAsync(u => u.OrderInformationGroupID == orderId);

        }
    }
}
