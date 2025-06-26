using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Example2.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public string? ReviewerName { get; set; }

        [Required]
        public string? Comment { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public DateTime ReviewDate { get; set; } = DateTime.Now;

        // Foreign key to Product
        [Required]
        public int ProductId { get; set; }
       
        public Product Product { get; set; }
    }
}
