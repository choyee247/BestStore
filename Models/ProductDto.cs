using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Example2.Models
{
    public class ProductDto
    {

        [Required, MaxLength(100)]
        public string ProductName { get; set; } = "";

        //[Required, MaxLength(100)]
        //public string Brand { get; set; } = "";

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Description { get; set; } = "";

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        public int? CategoryId { get; set; }

        public int? BrandId { get; set; }
        public List<SelectListItem>? Categories { get; set; }
        public List<SelectListItem>? Brands { get; set; }

        [DataType(DataType.Date)] 
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Entry Date")]
        public DateTime CreatedAt { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Update Date")]
        public DateTime UpdatedAt { get; set; }
    }
}
