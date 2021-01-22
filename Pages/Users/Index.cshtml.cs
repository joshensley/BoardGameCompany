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
using static BoardGameCompany.Pagination;

namespace BoardGameCompany.Pages.Users
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public string NameSort { get; set; }
        public string PhoneNumberSort { get; set; }
        public string EmailSort { get; set; }
        public string CurrentSort { get; set; }
        public string CurrentFilter { get; set; }

        public PaginatedList<ApplicationUser> ApplicationUserList { get; set; }

        public async Task OnGetAsync(string sortOrder, string searchString, string currentFilter, int? pageIndex)
        {
            CurrentSort = sortOrder;
            NameSort = sortOrder == "name_desc" ? "name_asc" : "name_desc";
            PhoneNumberSort = sortOrder == "phone_number_desc" ? "phone_number_asc" : "phone_number_desc";
            EmailSort = sortOrder == "email_desc" ? "email_asc" : "email_desc";

            if(searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            IQueryable<ApplicationUser> applicationUsersIQ = _db.ApplicationUser;

            if (!String.IsNullOrEmpty(searchString))
            {
                applicationUsersIQ = applicationUsersIQ.Where(a => a.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    applicationUsersIQ = applicationUsersIQ.OrderByDescending(a => a.Name);
                    break;
                case "name_asc":
                    applicationUsersIQ = applicationUsersIQ.OrderBy(a => a.Name);
                    break;
                case "phone_number_desc":
                    applicationUsersIQ = applicationUsersIQ.OrderByDescending(a => a.PhoneNumber);
                    break;
                case "phone_number_asc":
                    applicationUsersIQ = applicationUsersIQ.OrderBy(a => a.PhoneNumber);
                    break;
                case "email_desc":
                    applicationUsersIQ = applicationUsersIQ.OrderByDescending(a => a.Email);
                    break;
                case "email_asc":
                    applicationUsersIQ = applicationUsersIQ.OrderBy(a => a.Email);
                    break;
                default:
                    applicationUsersIQ = applicationUsersIQ.OrderBy(a => a.Name);
                    break;
            }

            int pageSize = 3;
            ApplicationUserList = await PaginatedList<ApplicationUser>.CreateAsync(
                applicationUsersIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
