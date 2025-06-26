using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Example2.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }

        [MaxLength(100)]
        public string ProductName { get; set; } = "";

        [MaxLength(100)]
        public string BrandName { get; set; } = "";

        [Precision(16, 2)]
        public decimal Price { get; set; }

        [MaxLength(100)]
        public string Description { get; set; } = "";

        [MaxLength(100)]
        public string ImageFileName { get; set; } = "";
        public DateTime CreateAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        [ForeignKey("CategoryId")]
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        [ForeignKey("BrandId")]
        public int? BrandId { get; set; }
        public Brand? Brand { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public DateTime CreatedAt { get; internal set; }
        public int StockQuantity { get; set; } = 10;
        public DateTime CreatedDate { get; set; } // For New Arrivals
        public double Rating { get; set; } // For Top Rated
        public bool IsCancelled { get; set; } // ✅ Add this if not present

        internal static List<Product> Where(Func<object, bool> value)
        {
            throw new NotImplementedException();
        }
    }
}
