using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Example2.Models
{
    public class Brand
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Brand name is required.")]
        public string? Name { get; set; }

        public List<BrandandCatView>? BrandandCatViews { get; set; }

        public List<Product> Products { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Please select at least one category.")]
        public List<int> SelectedCategoryIds { get; set; } = new();
    }
}
