using System;
using System.ComponentModel.DataAnnotations;

namespace Example2.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }
        public DateTime RegisteredAt { get; set; } = DateTime.Now;
    }
}
