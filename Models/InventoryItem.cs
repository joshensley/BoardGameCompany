using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace BoardGameCompany.Models
{
    public class InventoryItem
    {

        public enum ConditionOptions
        {
            New, Used
        }

        public int ID { get; set; }

        [Required]
        [Display(Name = "In Stock")]
        public bool InStock { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Received Date")]
        public DateTime ReceivedDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Shipped Date")]
        public DateTime ShippedDate { get; set; }

        [Required]
        [Display(Name = "Condition")]
        public ConditionOptions Condition { get; set; }

        public int BoardGameID { get; set; }
        public BoardGame BoardGame { get; set; }

        
    }
}