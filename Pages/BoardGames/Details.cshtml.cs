using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace BoardGameCompany.Pages.BoardGames
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public DetailsModel(ApplicationDbContext db)
        {
            _db = db;
        }

        // Sort
        public string ConditionSort { get; set; }
        public string ReceivedDateSort { get; set; }
        public string ShippedDateSort { get; set; }
        public string InStockSort { get; set; }
        public string CurrentSort { get; set; }

        // Filter
        public string InventoryFilter { get; set; }
        public string ConditionFilter { get; set; }

        // Properties
        public BoardGame BoardGame { get; set; }
        public PaginatedList<InventoryItem> InventoryItem { get; set; }

        public async Task OnGet(
            int? id,
            string sortOrder,
            string inventoryFilterString,
            string conditionFilterString,
            string currentFilter,
            int? pageIndex
            )
        {
            // Sort
            ConditionSort = sortOrder == "condition_desc" ? "condition_asc" : "condition_desc";
            InStockSort = sortOrder == "in_stock_desc" ? "in_stock_asc" : "in_stock_desc";
            ReceivedDateSort = sortOrder == "received_date_desc" ? "received_date_asc" : "received_date_desc";
            ShippedDateSort = sortOrder == "shipped_date_desc" ? "shipped_date_asc" : "shipped_date_desc";
            CurrentSort = sortOrder;

            // Filter
            InventoryFilter = inventoryFilterString;
            ConditionFilter = conditionFilterString;
            
            // Query
            BoardGame = await _db.BoardGame
                .Include(b => b.Brand)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            IQueryable<InventoryItem> inventoryItemsIQ = _db.InventoryItem.Where(m => m.BoardGameID == id);

            if (inventoryFilterString != null)
            {
                switch(inventoryFilterString)
                {
                    case "in_stock_all":
                        inventoryItemsIQ = inventoryItemsIQ.Where(i => i.InStock == true || i.InStock == false);
                        break;
                    case "in_stock_yes":
                        inventoryItemsIQ = inventoryItemsIQ.Where(i => i.InStock == true);
                        break;
                    case "in_stock_no":
                        inventoryItemsIQ = inventoryItemsIQ.Where(i => i.InStock == false);
                        break;
                    default:
                        inventoryItemsIQ = inventoryItemsIQ.Where(i => i.InStock == true || i.InStock == false);
                        break;
                }
            }

            if (conditionFilterString != null)
            {
                switch (conditionFilterString)
                {
                    case "all_conditions":
                        inventoryItemsIQ = inventoryItemsIQ.Where(
                           i => i.Condition == BoardGameCompany.Models.InventoryItem.ConditionOptions.New ||
                           i.Condition == BoardGameCompany.Models.InventoryItem.ConditionOptions.Used);
                        break;
                    case "new_condition":
                        inventoryItemsIQ = inventoryItemsIQ.Where(
                           i => i.Condition == BoardGameCompany.Models.InventoryItem.ConditionOptions.New);
                        break;
                    case "used_condition":
                        inventoryItemsIQ = inventoryItemsIQ.Where(
                           i => i.Condition == BoardGameCompany.Models.InventoryItem.ConditionOptions.Used);
                        break;
                    default:
                        inventoryItemsIQ = inventoryItemsIQ.Where(
                           i => i.Condition == BoardGameCompany.Models.InventoryItem.ConditionOptions.New || 
                           i.Condition == BoardGameCompany.Models.InventoryItem.ConditionOptions.Used);
                        break;  

                }
            }

            if (sortOrder != null || sortOrder != "")
            {
                switch (sortOrder)
                {
                    case "in_stock_asc":
                        inventoryItemsIQ = inventoryItemsIQ
                            .OrderBy(i => i.InStock)
                            .ThenBy(i => i.Condition)
                            .ThenByDescending(i => i.ReceivedDate);
                        break;
                    case "in_stock_desc":
                        inventoryItemsIQ = inventoryItemsIQ
                            .OrderByDescending(i => i.InStock)
                            .ThenBy(i => i.Condition)
                            .ThenByDescending(i => i.ReceivedDate);
                        break;
                    case "received_date_asc":
                        inventoryItemsIQ = inventoryItemsIQ.OrderBy(i => i.ReceivedDate);
                        break;
                    case "received_date_desc":
                        inventoryItemsIQ = inventoryItemsIQ.OrderByDescending(i => i.ReceivedDate);
                        break;
                    case "shipped_date_asc":
                        inventoryItemsIQ = inventoryItemsIQ.OrderBy(i => i.ShippedDate);
                        break;
                    case "shipped_date_desc":
                        inventoryItemsIQ = inventoryItemsIQ.OrderByDescending(i => i.ShippedDate);
                        break;
                    case "condition_asc":
                        inventoryItemsIQ = inventoryItemsIQ
                            .OrderBy(i => i.Condition)
                            .ThenByDescending(i => i.InStock)
                            .ThenByDescending(i => i.ReceivedDate);
                        break;
                    case "condition_desc":
                        inventoryItemsIQ = inventoryItemsIQ
                            .OrderByDescending(i => i.Condition)
                            .ThenByDescending(i => i.InStock)
                            .ThenByDescending(i => i.ReceivedDate);
                        break;
                    default:
                        inventoryItemsIQ = inventoryItemsIQ
                            .OrderByDescending(i => i.InStock)
                            .ThenBy(i => i.Condition)
                            .ThenByDescending(i => i.ReceivedDate);
                        break;
                }
            }

            int pageSize = 3;
            // InventoryItem = await inventoryItemsIQ.AsNoTracking().ToListAsync();
            InventoryItem = await PaginatedList<InventoryItem>.CreateAsync(
                inventoryItemsIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
