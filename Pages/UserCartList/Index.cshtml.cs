using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoardGameCompany.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BoardGameCompany.Models;
using Microsoft.EntityFrameworkCore;
using static BoardGameCompany.Pagination;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BoardGameCompany.Pages.UserCartList
{
    [Authorize(Roles = "Admin,Customer")]
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

        public PaginatedList<UserCart> UserCart { get; set; }
        public IList<UserCart> UserCartList { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal GrandTotal { get; set; }
        public int TotalQuantity { get; set; }
        public string ErrorMessage { get; set; }
        public int ErrorBoardGameId { get; set; }

        public async Task OnGet(
            int boardGameId,
            int? currentStock,
            bool? changeQuantityError = false)
        {
            IQueryable<UserCart> userCartIQ = null;
            decimal subTotalIQ = 0;
            int totalQuantity = 0;
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);

                userCartIQ = _db.UserCart
                   .Where(u => u.ApplicationUser.Id == user.Id)
                   .Include(u => u.ApplicationUser)
                   .Include(u => u.BoardGame)
                   .ThenInclude(b => b.Brand);

                subTotalIQ = _db.UserCart
                    .Where(u => u.ApplicationUser.Id == user.Id)
                    .Include(u => u.BoardGame)
                    .Sum(b => b.BoardGame.Price * b.Quantity);

                totalQuantity = _db.UserCart
                    .Where(u => u.ApplicationUser.Id == user.Id)
                    .Include(u => u.BoardGame)
                    .Sum(b => b.Quantity);

            }

            TotalQuantity = totalQuantity;

            UserCartList = await userCartIQ.ToListAsync();
            SubTotal = Math.Round(subTotalIQ, 2);

            var salesTaxRate = 0.0625;
            Tax = Math.Round(SubTotal * Convert.ToDecimal(salesTaxRate), 2);

            GrandTotal = Math.Round((SubTotal + Tax), 2);

            if (changeQuantityError.GetValueOrDefault())
            {
                ErrorBoardGameId = boardGameId;
                ErrorMessage = $"Only {currentStock} in stock";
                
            }

        }

        public async Task<IActionResult> OnPostDeleteAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userCartItem = await _db.UserCart.FindAsync(id);

            if (userCartItem == null)
            {
                return NotFound();
            }

            _db.UserCart.Remove(userCartItem);
            await _db.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostEditAsync(
            int? id,
            int? boardGameId,
            string change, 
            decimal price)
        {
            if (id == null || change == null || boardGameId == null)
            {
                return NotFound();
            }

            var userCartToUpdate = await _db.UserCart.FindAsync(id);

            if (userCartToUpdate == null)
            {
                return NotFound();
            }

            int currentStock = _db.InventoryItem
                .Where(i => i.BoardGameID == boardGameId)
                .Where(i => i.InStock == true)
                .Count();

            if (await TryUpdateModelAsync<UserCart>(
                userCartToUpdate,
                "usercart",
                u => u.Quantity,
                u => u.TotalPrice))
            {

                switch (change)
                {
                    case "increase":

                        if (userCartToUpdate.Quantity >= currentStock)
                        {
                            return RedirectToPage("./Index", new { 
                                changeQuantityError = true, 
                                currentStock,
                                boardGameId
                            });
                        }

                        userCartToUpdate.Quantity += 1;
                        break;
                    case "decrease":

                        if(userCartToUpdate.Quantity > 1)
                        {
                            userCartToUpdate.Quantity -= 1;
                        }
                        break;
                    default:
                        userCartToUpdate.Quantity += 0;
                        break;
                }

                userCartToUpdate.TotalPrice = price * userCartToUpdate.Quantity;

                await _db.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();

        }
    }
}
