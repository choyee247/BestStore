using System;
using System.ComponentModel.DataAnnotations;

namespace Example2.Models
{
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        public Order Order { get; set; }

        [Required]
        [Display(Name = "Card Holder Name")]
        public string CardHolder { get; set; }

        [Required]
        [CreditCard]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; }

        [Required]
        [Display(Name = "Expiry Date")]
        public string Expiry { get; set; }

        [Required]
        public string CVV { get; set; }

        public DateTime PaidAt { get; set; } = DateTime.Now;
    }
}
