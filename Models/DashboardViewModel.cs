using System.ComponentModel.DataAnnotations.Schema;

namespace Example2.Models
{
    [NotMapped]
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public decimal AveragePrice { get; set; }

        //public int TopRated { get; set; }
        //public int NewArrivals { get; set; }
        //public int TotalOrders { get; set; }
        //public int Completed { get; set; }
        //public int Pending { get; set; }
        //public int Cancelled { get; set; }

        public List<Product> TopRatedProducts { get; set; }
        public List<Product> NewArrivalProducts { get; set; }
        public List<Order> PendingOrders { get; set; }
        public List<Order> CancelledOrders { get; set; }
        public ProductSummary ProductSummary { get; set; }
        public OrderSummary OrderSummary { get; set; }
    }
}
