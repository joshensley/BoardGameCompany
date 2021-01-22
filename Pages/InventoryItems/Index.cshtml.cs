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

namespace BoardGameCompany.Pages.InventoryItems
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        // Filter
        public string CurrentFilter { get; set; }

        // Sort
        public string IDSort { get; set; }
        public string TitleSort { get; set; }
        public string InStockSort { get; set; }
        public string ReceivedDateSort { get; set; }
        public string ShippedDateSort { get; set; }
        public string ConditionSort { get; set; }
        public string CurrentSort { get; set; }

        // Properties
        public PaginatedList<InventoryItem> InventoryItems { get; set; }

        public async Task OnGetAsync(
            int? pageIndex,
            string sortOrder,
            string searchString,
            string currentFilter)
        {

            // Sort
            CurrentSort = sortOrder;
            IDSort = sortOrder == "id_desc" ? "id_asc" : "id_desc";
            TitleSort = sortOrder == "title_desc" ? "title_asc" : "title_desc";
            InStockSort = sortOrder == "in_stock_desc" ? "in_stock_asc" : "in_stock_desc";
            ReceivedDateSort = sortOrder == "received_date_desc" ? "received_date_asc" : "received_date_desc";
            ShippedDateSort = sortOrder == "shipped_date_desc" ? "shipped_date_asc" : "shipped_date_desc";
            ConditionSort = sortOrder == "condition_desc" ? "condition_asc" : "condition_desc";

            if(searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            // Filter
            CurrentFilter = searchString;

            IQueryable<InventoryItem> inventoryItemsIQ = _db.InventoryItem.Include(i => i.BoardGame);

            if (!String.IsNullOrEmpty(searchString))
            {
                inventoryItemsIQ = inventoryItemsIQ.Where(b => b.BoardGame.Title.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "id_desc":
                    inventoryItemsIQ = inventoryItemsIQ.OrderByDescending(i => i.BoardGame.ID);
                    break;
                case "id_asc":
                    inventoryItemsIQ = inventoryItemsIQ.OrderBy(i => i.BoardGame.ID);
                    break;
                case "title_desc":
                    inventoryItemsIQ = inventoryItemsIQ.OrderByDescending(i => i.BoardGame.Title);
                    break;
                case "title_asc":
                    inventoryItemsIQ = inventoryItemsIQ.OrderBy(i => i.BoardGame.Title);
                    break;
                case "in_stock_desc":
                    inventoryItemsIQ = inventoryItemsIQ
                        .OrderByDescending(i => i.InStock)
                        .ThenBy(i => i.BoardGame.Title);
                    break;
                case "in_stock_asc":
                    inventoryItemsIQ = inventoryItemsIQ
                        .OrderBy(i => i.InStock)
                        .ThenBy(i => i.BoardGame.Title);
                    break;
                case "received_date_desc":
                    inventoryItemsIQ = inventoryItemsIQ
                        .OrderByDescending(i => i.ReceivedDate)
                        .ThenBy(i => i.BoardGame.Title);
                    break;
                case "received_date_asc":
                    inventoryItemsIQ = inventoryItemsIQ
                        .OrderBy(i => i.ReceivedDate)
                        .ThenBy(i => i.BoardGame.Title);
                    break;
                case "shipped_date_desc":
                    inventoryItemsIQ = inventoryItemsIQ
                        .OrderByDescending(i => i.ShippedDate)
                        .ThenBy(i => i.BoardGame.Title);
                    break;
                case "shipped_date_asc":
                    inventoryItemsIQ = inventoryItemsIQ
                        .OrderBy(i => i.ShippedDate)
                        .ThenBy(i => i.BoardGame.Title);
                    break;
                case "condition_desc":
                    inventoryItemsIQ = inventoryItemsIQ
                        .OrderByDescending(i => i.Condition)
                        .ThenBy(i => i.BoardGame.Title);
                    break;
                case "condition_asc":
                    inventoryItemsIQ = inventoryItemsIQ
                        .OrderBy(i => i.Condition)
                        .ThenBy(i => i.BoardGame.Title);
                    break;
                default:
                    inventoryItemsIQ = inventoryItemsIQ.OrderBy(i => i.BoardGame.Title);
                    break;
            }

            int pageSize = 5;
            InventoryItems = await PaginatedList<InventoryItem>.CreateAsync(
                inventoryItemsIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
