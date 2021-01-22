using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class IndexModel : PageModel
    {
        public readonly ApplicationDbContext _db;
        public readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // Cart
        public IList<UserCart> UserCartList { get; set; }

        [BindProperty]
        public decimal SubTotal { get; set; }
        [BindProperty]
        public decimal Tax { get; set; }
        [BindProperty]
        public decimal Shipping { get; set; }
        [BindProperty]
        public decimal GrandTotal { get; set; }
        public UserShippingInformation UserShippingInformation { get; set; }
        public UserBillingInformation UserBillingInformation { get; set; }
        public UserOrderInformation UserOrderInformation { get; set; }
        public UserOrderInformationPrice UserOrderInformationPrice { get; set; }

        // Cart
        public async Task OnGetAsync()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);

            // Cart
            IQueryable<UserCart> userCartIQ = _db.UserCart
                .Where(u => u.ApplicationUser.Id == user.Id)
                .Include(u => u.ApplicationUser)
                .Include(u => u.BoardGame)
                .ThenInclude(b => b.Brand);

            var subTotalIQ = _db.UserCart
                .Where(u => u.ApplicationUser.Id == user.Id)
                .Include(u => u.BoardGame)
                .Sum(b => b.BoardGame.Price * b.Quantity);

            var totalQuantity = _db.UserCart
                .Where(u => u.ApplicationUser.Id == user.Id)
                .Include(u => u.BoardGame)
                .Sum(b => b.Quantity);

            UserCartList = await userCartIQ.ToListAsync();

            SubTotal = Math.Round(subTotalIQ, 2);

            var salesTaxRate = 0.0625;
            Tax = Math.Round((SubTotal * Convert.ToDecimal(salesTaxRate)), 2);

            // Shipping
            UserShippingInformation = await _db.UserShippingInformation
                .FirstOrDefaultAsync(u => u.ApplicationUserId == user.Id);

            // Billing
            UserBillingInformation = await _db.UserBillingInformation
                .FirstOrDefaultAsync(u => u.ApplicationUserId == user.Id);

            if(UserShippingInformation != null)
            {
                Shipping = 5.00M;
                GrandTotal = Math.Round((SubTotal + Tax + Shipping), 2);
            }
            else
            {   
                GrandTotal = Math.Round((SubTotal + Tax), 2);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);

            var userCart = await _db.UserCart.Where(u => u.ApplicationUserId == user.Id).ToListAsync();

            foreach (var item in userCart)
            {
                var inventoryItems = await _db.InventoryItem
                    .Where(i => i.BoardGameID == item.BoardGameID)
                    .Where(i => i.InStock == true)
                    .ToListAsync();

                if (inventoryItems == null)
                {
                    return NotFound();
                }

                
                if (inventoryItems.Count < item.Quantity)
                {
                    // Need to redirect to Checkout page with error
                    return NotFound();
                }

            }

            // Charge credit card
            // if charging credit card fails then return stock items that were "false" back to "true"
            // return to checkout page with credit card error



            // Change items in inventory to not in stock
            // if can't change in stock from "true" to "false" return to checkout page with error
            foreach (var item in userCart)
            {
                var inventoryItems = await _db.InventoryItem
                    .Where(i => i.InStock == true)
                    .Where(i => i.BoardGameID == item.BoardGameID)
                    .ToListAsync();

                for (int i = 0; i < item.Quantity; i++)
                {
                    inventoryItems[i].InStock = false;
                }

                await _db.SaveChangesAsync();

            }

            // Create order information based off the user cart
            // Send user order comfirmation via email
            var rand = new Random();
            var uid = rand.Next(100000, 1000000);
;
            foreach (var item in userCart)
            {
                var emptyUserOrderInformation = new UserOrderInformation();

                await TryUpdateModelAsync<UserOrderInformation>(
                    emptyUserOrderInformation,
                    "userorderinformation",
                    u => u.OrderDate,
                    u => u.TotalPrice,
                    u => u.OrderInformationGroupID,
                    u => u.BoardGameID,
                    u => u.Quantity,
                    
                    u => u.ApplicationUserId);
                {
                    emptyUserOrderInformation.OrderDate = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd"));
                    emptyUserOrderInformation.TotalPrice = item.TotalPrice;
                    emptyUserOrderInformation.OrderInformationGroupID = uid;
                    emptyUserOrderInformation.BoardGameID = item.BoardGameID;
                    emptyUserOrderInformation.Quantity = item.Quantity;
                    emptyUserOrderInformation.ApplicationUserId = item.ApplicationUserId;

                    _db.UserOrderInformation.Add(emptyUserOrderInformation);
                    await _db.SaveChangesAsync();
                }
            }

            // Add pricing information based off OrderInformationGroupID number
            var emptyUserOrderInformationPrice = new UserOrderInformationPrice();

            await TryUpdateModelAsync<UserOrderInformationPrice>(
                emptyUserOrderInformationPrice,
                "userorderinformationprice",
                u => u.OrderInformationGroupID,
                u => u.Subtotal,
                u => u.Tax,
                u => u.Shipping,
                u => u.GrandTotal);
            {
                emptyUserOrderInformationPrice.OrderInformationGroupID = uid;

                _db.UserOrderInformationPrice.Add(emptyUserOrderInformationPrice);
                await _db.SaveChangesAsync();
            }

            // Clear the user cart
            _db.UserCart.RemoveRange(userCart);
            await _db.SaveChangesAsync();


            // redirect to order confirmation page
            return RedirectToPage("./ConfirmationPage", new { orderId = uid });

        }
    }
}
