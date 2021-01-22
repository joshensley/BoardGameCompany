using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompany.Models
{
    public class UserOrderInformationPrice
    {
        public int ID { get; set; }

        [Required]
        public int OrderInformationGroupID { get; set; }

        [Required]
        public decimal Subtotal { get; set; }

        [Required]
        public decimal Tax { get; set; }

        [Required]
        public decimal Shipping { get; set; }

        [Required]
        public decimal GrandTotal { get; set; }
    }
}
