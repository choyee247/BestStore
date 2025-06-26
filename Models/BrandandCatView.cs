using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Example2.Models
{
    public class BrandandCatView
    {
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public int BrandId { get; set; }
        public Brand? Brand { get; set; }

        //public List<SelectListItem>? Categories { get; set; }
        //public List<SelectListItem>? Brands { get; set; }
    }
}
