using System.ComponentModel.DataAnnotations;

namespace Example2.Models
{
    public class PaymentViewModel
    {
        public int OrderId { get; set; }

        [Required]
        public string CardHolder { get; set; }

        [Required]
        [CreditCard]
        public string CardNumber { get; set; }

        [Required]
        public string Expiry { get; set; }

        [Required]
        public string CVV { get; set; }

        public decimal Amount { get; set; }
    }
}
