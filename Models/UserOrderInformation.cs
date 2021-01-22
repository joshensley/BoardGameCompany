using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompany.Models
{
    public class UserOrderInformation
    {
        public int ID { get; set; }

        [Required]
        public int OrderInformationGroupID { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        [Range(1, Int32.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }

        public int? BoardGameID { get; set; }
        public BoardGame BoardGame { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
