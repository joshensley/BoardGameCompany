using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompany.Models
{
    public class UserShippingInformation
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Name")]
        [StringLength(256)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Country")]
        [StringLength(256)]
        public string Country { get; set; }

        [Required]
        [Display(Name = "Address Line 1")]
        [StringLength(256)]
        public string Address1 { get; set; }

        [Display(Name = "Address Line 2")]
        [StringLength(256)]
        public string Address2 { get; set; }

        [Required]
        [Display(Name = "City")]
        [StringLength(256)]
        public string City { get; set; }

        [Required]
        [Display(Name = "State")]
        [StringLength(256)]
        public string State { get; set; }

        [Required]
        [Display(Name = "Postal Code")]
        [StringLength(5, MinimumLength = 5)]
        public string PostalCode { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
