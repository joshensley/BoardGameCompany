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
using static BoardGameCompany.Pagination;

namespace BoardGameCompany.Pages.OrderHistory
{
    [Authorize(Roles = "Admin,Customer")]
    public class IndexModel : PageModel
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [BindProperty]
        public PaginatedList<UserOrderInformation> UserOrderInformation { get; set; }

        public async Task OnGetAsync(int? pageIndex)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);

            IQueryable<UserOrderInformation> userOrderInformationIQ = _db.UserOrderInformation
                 .Where(u => u.ApplicationUserId == user.Id)
                 .OrderByDescending(u => u.OrderDate)
                 .ThenByDescending(u => u.OrderInformationGroupID)
                 .Include(u => u.BoardGame)
                     .ThenInclude(u => u.Brand);

            int pageSize = 4;
            UserOrderInformation = await PaginatedList<UserOrderInformation>.CreateAsync(
                userOrderInformationIQ.AsNoTracking(), pageIndex ?? 1, pageSize);

            //UserOrderInformation = await userOrderInformationIQ.AsNoTracking().ToListAsync();

        }
    }
}
