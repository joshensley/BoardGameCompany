using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompany.Models
{
    public class PaymentInformation
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Name On Card")]
        public string NameOnCard { get; set; }

        [Required]
        [Display(Name = "Credit Card Number")]
        [CreditCard]
        public string CreditCardNumber { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "", ApplyFormatInEditMode = true)]
        [Display(Name = "Expiration Month")]
        public DateTime ExpirationMonth { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "", ApplyFormatInEditMode = true)]
        [Display(Name = "Expiration Year")]
        public DateTime ExpirationYear { get; set; }

        [Required]
        [Display(Name = "CVV")]
        public string CVV { get; set; }
    }
}
